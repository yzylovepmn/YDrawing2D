using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace YOpenGL._3D
{
    public class MouseEventHandler : IDisposable
    {
        public MouseEventHandler(GLPanel3D panel)
        {
            _panel = panel;
            _AttachEvents();
        }

        private GLPanel3D _panel;

        private PointF _lastPoint;
        private Point3F? _lastPoint3D;

        private PointF _mouseDownPoint;
        private Point3F? _mouseDownPoint3D;

        private void _AttachEvents()
        {
            _panel.MouseDown += _OnMouseDown;
            _panel.MouseUp += _OnMouseUp;
            _panel.MouseMove += _OnMouseMove;
            _panel.MouseWheel += _OnMouseWheel;
        }

        private void _OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _panel.Focus();
            _panel.CaptureMouse();

            _mouseDownPoint = _panel.GetPosition();
            _mouseDownPoint3D = _panel.PointInWpfToPoint3D(_mouseDownPoint);

            _lastPoint = _mouseDownPoint;
            _lastPoint3D = _mouseDownPoint3D;
        }

        private void _OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _panel.ReleaseMouseCapture();
        }

        private void _OnMouseMove(object sender, MouseEventArgs e)
        {
            var mouseMovePoint = _panel.GetPosition();
            var mouseMovePoint3D = _panel.PointInWpfToPoint3D(mouseMovePoint);

            var rotateStatus = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Translate
                if (Keyboard.Modifiers == ModifierKeys.Shift && _panel.IsTranslateEnable)
                {
                    if (mouseMovePoint3D.HasValue && _lastPoint3D.HasValue)
                    {
                        _panel.Translate((_lastPoint3D - mouseMovePoint3D).Value);
                        mouseMovePoint3D = _lastPoint3D; // mouseMovePoint 所对应的 mouseMovePoint3D 保持不变
                    }
                }
                // Rotate
                if (Keyboard.Modifiers == ModifierKeys.Control && _panel.IsRotateEnable)
                {
                    rotateStatus = true;
                    if (!_isRotating)
                    {
                        _isRotating = true;
                        _InitRotateParameters(mouseMovePoint, mouseMovePoint3D);
                    }
                    Rotate(_lastPoint, mouseMovePoint, _rotationPoint3D);
                }
            }

            _isRotating = rotateStatus;
            _lastPoint = mouseMovePoint;
            _lastPoint3D = mouseMovePoint3D;
        }

        private void _OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_lastPoint3D.HasValue && _panel.IsZoomEnable)
                _panel.Zoom(-e.Delta / 2000.0f, _lastPoint3D.Value);
        }

        #region Rotate
        private bool _isRotating;
        internal Vector3F _rotationAxisX;
        internal Vector3F _rotationAxisY;
        private PointF _rotationPoint;
        private Point3F _rotationPoint3D;

        public void Rotate(PointF p0, PointF p1, Point3F rotateAround)
        {
            var camera = _panel.Camera;
            switch (camera.RotationMode)
            {
                case CameraRotationMode.Trackball:
                    _panel.RotateTrackball(p0, p1, rotateAround);
                    break;
                case CameraRotationMode.Turntable:
                    _panel.RotateTurntable(p1 - p0, rotateAround);
                    break;
                case CameraRotationMode.Turnball:
                    _InitTurnballRotationAxes(p0);
                    _panel.RotateTurnball(p0, p1, rotateAround);
                    break;
            }

            if (Math.Abs(camera.UpDirection.Length - 1) > 1e-8)
            {
                var upDirection = camera.UpDirection;
                upDirection.Normalize();
                camera.SetViewParameters(camera.Position, camera.LookDirection, upDirection);
                _panel.Refresh();
            }
        }

        private void _InitRotateParameters(PointF mouseMovePoint, Point3F? mouseMovePoint3D)
        {
            _rotationPoint = new PointF(_panel.ViewWidth / 2, _panel.ViewHeight / 2);
            _rotationPoint3D = _panel.Camera.Target;

            switch (_panel.Camera.Mode)
            {
                case CameraMode.WalkAround:
                    _rotationPoint = mouseMovePoint;
                    _rotationPoint3D = _panel.Camera.Position;
                    break;
                default:
                    if (_panel.RotateAroundMouse && mouseMovePoint3D.HasValue)
                    {
                        _rotationPoint = mouseMovePoint;
                        _rotationPoint3D = mouseMovePoint3D.Value;
                    }
                    break;
            }

            switch (_panel.Camera.RotationMode)
            {
                case CameraRotationMode.Trackball:
                    break;
                case CameraRotationMode.Turntable:
                    break;
                case CameraRotationMode.Turnball:
                    _InitTurnballRotationAxes(mouseMovePoint);
                    break;
            }
        }

        private void _InitTurnballRotationAxes(PointF p1)
        {
            double fx = p1.X / _panel.ViewWidth;
            double fy = p1.Y / _panel.ViewHeight;

            Vector3F up = _panel.Camera.UpDirection;
            Vector3F dir = _panel.Camera.LookDirection;
            dir.Normalize();

            Vector3F right = Vector3F.CrossProduct(dir, up);
            right.Normalize();

            _rotationAxisX = up;
            _rotationAxisY = right;
            if (fy > 0.8 || fy < 0.2)
            {
                // delta.Y = 0;
            }

            if (fx > 0.8)
            {
                // delta.X = 0;
                _rotationAxisY = dir;
            }

            if (fx < 0.2)
            {
                // delta.X = 0;
                _rotationAxisY = -dir;
            }
        }
        #endregion

        private void _DetachEvents()
        {
            _panel.MouseDown -= _OnMouseDown;
            _panel.MouseUp -= _OnMouseUp;
            _panel.MouseMove -= _OnMouseMove;
            _panel.MouseWheel -= _OnMouseWheel;
        }

        public void Dispose()
        {
            _DetachEvents();
            _panel = null;
        }
    }
}