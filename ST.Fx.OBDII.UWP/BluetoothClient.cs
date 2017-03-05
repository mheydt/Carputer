using ST.Fx.Debug.Tracer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ST.Fx.OBDII
{
    public class BluetoothClient : IOBDIITransport
    {
        private RfcommDeviceService _service;
        private StreamSocket _socket;
        private bool _connected;
        private DataWriter _dataWriterObject;
        private DataReader _dataReaderObject;
        private bool _running;

        public object ObdShare { get; private set; }

        public async Task<bool> InitAsync(CancellationToken cancellation = default(CancellationToken))
        {
            var deviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            var numDevices = deviceInfoCollection.Count();
            DeviceInformation device = null;
            foreach (var info in deviceInfoCollection)
            {
                if (info.Name.ToLower().Contains("obd"))
                {
                    device = info;
                }
            }
            if (device == null) return false;

            _service = await RfcommDeviceService.FromIdAsync(device.Id);

            // Disposing the socket with close it and release all resources associated with the socket
            _socket?.Dispose();

            _socket = new StreamSocket();
            try
            {
                // Note: If either parameter is null or empty, the call will throw an exception
                await _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName);
                _connected = true;
            }
            catch (Exception ex)
            {
                _connected = false;
                Tracer.writeLine("Connect:" + ex.Message);
                return false;
            }

            // If the connection was successful, the RemoteAddress field will be populated
            if (_connected)
            {
                var msg = String.Format("Connected to {0}!", _socket.Information.RemoteAddress.DisplayName);
                Tracer.writeLine(msg);

                _dataWriterObject = new DataWriter(_socket.OutputStream);
                _dataReaderObject = new DataReader(_socket.InputStream);
            }

            return true;
        }

        public async Task ShutdownAsync(CancellationToken cancellation = default(CancellationToken))
        {
            _running = false;
            _dataReaderObject?.Dispose();
            _dataReaderObject?.Dispose();

            _dataReaderObject = null;
            _dataWriterObject = null;

            if (_socket != null)
            {
                try
                {
                    await _socket.CancelIOAsync();
                    _socket.Dispose();
                }
                catch (Exception ex)
                {
                    Tracer.writeLine(ex.Message);
                }
                _socket = null;
            }
        }

        public async Task<string> ExecuteCommand(string command, string terminator = ">", CancellationToken cancellation = default(CancellationToken))
        {
            Tracer.writeLine($"ExecuteCommand: {command}");

            await WriteAsync(command, cancellation);
            var response = await listenForResponse(terminator, cancellation);

            Tracer.writeLine($"ExecuteCommand out: {response}");

            return response;
        }

        private async Task<string> listenForResponse(string terminator = ">", CancellationToken cancellation = default(CancellationToken))
        {
            Tracer.writeLine("listenForResponse in");

            var response = "";

            while (true)
            {
                Tracer.writeLine("Waiting for more data");
                var r = await ReadAsync(terminator, cancellation);
                Tracer.writeLine($"listenForResponse: Read: {r}");

                r = r.Replace("SEARCHING...", "").Replace("\r\n", " ").Replace("\r", " ");

                response = response + r;
                response = response.Trim();

                if (r.EndsWith(terminator)) break;
            }

            Tracer.writeLine("listenForResponse out: " + response);

            return response;
        }

        private async Task WriteAsync(string msg, CancellationToken cancellation = default(CancellationToken))
        {
            // Load the text from the sendText input text box to the dataWriter object
            _dataWriterObject.WriteString(msg);

            // Launch an async task to complete the write operation
            var bytesWritten = await _dataWriterObject.StoreAsync().AsTask(cancellation);
            if (bytesWritten > 0)
            {
                var statusText = msg + ", ";
                statusText += bytesWritten.ToString();
                statusText += " bytes written successfully!";
                Tracer.writeLine(statusText);
            }
        }

        private async Task<string> ReadAsync(string terminator = ">", CancellationToken cancellation = default(CancellationToken))
        {
            var ret = await ReadAsyncRaw(cancellation);
            while (!ret.Trim().EndsWith(terminator))
            {
                var tmp = await ReadAsyncRaw(cancellation);
                ret = ret + tmp;
            }
            return ret;
        }

        private async Task<string> ReadAsyncRaw(CancellationToken cancellation = default(CancellationToken))
        {
            var readBufferLength = 1024u;

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            var bytesRead = await _dataReaderObject.LoadAsync(readBufferLength).AsTask(cancellation);
            if (bytesRead > 0)
            {
                try
                {
                    var recvdtxt = _dataReaderObject.ReadString(bytesRead);
                    Tracer.writeLine(recvdtxt);
                    return recvdtxt;
                }
                catch (Exception ex)
                {
                    Tracer.writeLine("ReadAsync: " + ex.Message);
                    return "";
                }
            }
            return "";
        }
    }
}
