using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace Carputer.UWP
{
    public abstract class InputProcessor
    {
        protected Windows.UI.Input.GestureRecognizer _gestureRecognizer;

        // Element being manipulated
        protected Windows.UI.Xaml.FrameworkElement _target;
        public Windows.UI.Xaml.FrameworkElement Target
        {
            get { return _target; }
        }

        // Reference element that contains the coordinate space used for expressing transformations 
        // during manipulation, usually the parent element of Target in the UI tree
        protected Windows.UI.Xaml.Controls.Canvas _reference;
        public Windows.UI.Xaml.FrameworkElement Reference
        {
            get { return _reference; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">
        /// Manipulation target.
        /// </param>
        /// <param name="reference">
        /// Element that contains the coordinate space used for expressing transformations
        /// during manipulations, usually the parent element of Target in the UI tree.
        /// </param>
        /// <remarks>
        /// Transformations during manipulations cannot be expressed in the coordinate space of the manipulation target.
        /// Thus <paramref name="element"/> and <paramref name="reference"/> must be different. Usually <paramref name="reference"/>
        /// will be an ancestor of <paramref name="element"/> in the UI tree.
        /// </remarks>
        internal InputProcessor(Windows.UI.Xaml.FrameworkElement element, Windows.UI.Xaml.Controls.Canvas reference)
        {
            this._target = element;
            this._reference = reference;

            // Setup pointer event handlers for the element.
            // They are used to feed the gesture recognizer.    
            this._target.PointerCanceled += OnPointerCanceled;
            this._target.PointerMoved += OnPointerMoved;
            this._target.PointerPressed += OnPointerPressed;
            this._target.PointerReleased += OnPointerReleased;
            this._target.PointerWheelChanged += OnPointerWheelChanged;

            // Create the gesture recognizer
            this._gestureRecognizer = new Windows.UI.Input.GestureRecognizer();
            this._gestureRecognizer.GestureSettings = Windows.UI.Input.GestureSettings.None;
        }

        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Route the events to the gesture recognizer.
            // All intermediate points are passed to the gesture recognizer in
            // the coordinate system of the reference element.
            this._gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints(this._reference));

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }
    }
}
