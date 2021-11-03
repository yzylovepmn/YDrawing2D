using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL._3D
{
    public enum CameraType
    {
        Perspective,
        Orthographic,
    }

    public enum CameraMode
    {
        /// <summary>
        /// Orbits around a point (fixed target position, move closer target when zooming).
        /// </summary>
        Inspect,

        /// <summary>
        /// Walk around (fixed camera position, move in cameradirection when zooming).
        /// </summary>
        WalkAround,

        /// <summary>
        /// Fixed camera target, change FOV when zooming.
        /// </summary>
        FixedPosition
    }

    public enum CameraRotationMode
    {
        /// <summary>
        /// Turnball using three axes (look direction, right direction and up direction (on the left/right edges)).
        /// </summary>
        Turnball,

        /// <summary>
        /// Turntable is constrained to two axes of rotation (model up and right direction)
        /// </summary>
        Turntable,

        /// <summary>
        /// Using a virtual trackball.
        /// </summary>
        Trackball
    }

    public class Camera : IDisposable
    {
        internal Camera(GLPanel3D viewPort, CameraType type
            , float width, float height, float nearPlaneDistance, float farPlaneDistance
            , Point3F position, Vector3F lookDirection, Vector3F upDirection)
        {
            _viewPort = viewPort;
            _type = type;

            SetOrthographicParameters(width, height, nearPlaneDistance, farPlaneDistance);
            SetPerspectiveParameters(45, width / height, nearPlaneDistance, farPlaneDistance);

            SetViewParameters(position, lookDirection, upDirection);
        }

        public GLPanel3D ViewPort { get { return _viewPort; } }
        private GLPanel3D _viewPort;

        public event EventHandler PropertyChanged = delegate { };

        public CameraType Type 
        { 
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    _UpdateProjectionMatrix();
                }
            }
        }
        private CameraType _type;

        public CameraMode Mode { get { return _mode; } set { _mode = value; } }
        private CameraMode _mode;

        public CameraRotationMode RotationMode { get { return _rotationMode; } set { _rotationMode = value; } }
        private CameraRotationMode _rotationMode;

        public float Width { get { return _width; } }
        private float _width;

        public float Height { get { return _height; } }
        private float _height;

        public float FieldOfView { get { return _fieldOfView; } }
        private float _fieldOfView;

        public float Aspect { get { return _aspect; } }
        private float _aspect;

        //public float Left { get { return _left; } }
        //private float _left;

        //public float Right { get { return _right; } }
        //private float _right;

        //public float Top { get { return _top; } }
        //private float _top;

        //public float Bottom { get { return _bottom; } }
        //private float _bottom;

        public float NearPlaneDistance { get { return _nearPlaneDistance; } }
        private float _nearPlaneDistance;

        public float FarPlaneDistance { get { return _farPlaneDistance; } }
        private float _farPlaneDistance;

        public Point3F Position { get { return _position; } }
        private Point3F _position;

        public Vector3F LookDirection { get { return _lookDirection; } }
        private Vector3F _lookDirection;

        public Point3F Target { get { return _position + _lookDirection; } }

        public Vector3F UpDirection { get { return _upDirection; } }
        private Vector3F _upDirection;

        public Matrix3F ProjectionMatrix { get { return _projectionMatrix; } }
        private Matrix3F _projectionMatrix;

        public Matrix3F ViewMatrix { get { return _viewMatrix; } }
        private Matrix3F _viewMatrix;

        internal Matrix3F TotalTransform { get { return _totalTransform; } }
        private Matrix3F _totalTransform;

        internal Matrix3F? TotalTransformReverse { get { return _totalTransformReverse; } }
        private Matrix3F? _totalTransformReverse;

        private bool _isLocked;

        internal void Lock()
        {
            _isLocked = true;
        }

        internal void UnLock()
        {
            _isLocked = false;
        }

        public void SetPerspectiveParameters(float fieldOfView)
        {
            SetPerspectiveParameters(fieldOfView, _aspect, _nearPlaneDistance, _farPlaneDistance);
        }

        public void SetPerspectiveParameters(float fieldOfView, float aspect)
        {
            SetPerspectiveParameters(fieldOfView, aspect, _nearPlaneDistance, _farPlaneDistance);
        }

        public void SetPerspectiveParameters(float fieldOfView, float aspect, float nearPlaneDistance, float farPlaneDistance)
        {
            _fieldOfView = fieldOfView;
            _aspect = aspect;
            _nearPlaneDistance = nearPlaneDistance;
            _farPlaneDistance = farPlaneDistance;
            _width = _aspect * _height;

            _UpdateProjectionMatrix();
        }

        public void SetOrthographicParameters(float width, float height)
        {
            SetOrthographicParameters(width, height, _nearPlaneDistance, _farPlaneDistance);
        }

        public void SetOrthographicParameters(float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            _width = Math.Max(1, width);
            _height = Math.Max(1, height);
            _nearPlaneDistance = nearPlaneDistance;
            _farPlaneDistance = farPlaneDistance;
            _aspect = _width / _height;

            _UpdateProjectionMatrix();
        }

        public void SetViewParameters(Point3F position)
        {
            SetViewParameters(position, _lookDirection);
        }

        public void SetViewParameters(Point3F position, Vector3F lookDirection)
        {
            SetViewParameters(position, lookDirection, _upDirection);
        }

        public void SetViewParameters(Point3F position, Vector3F lookDirection, Vector3F upDirection)
        {
            _position = position;
            _lookDirection = lookDirection;
            _upDirection = upDirection;

            _UpdateViewMatrix();
        }

        private void _UpdateProjectionMatrix()
        {
            switch (_type)
            {
                case CameraType.Perspective:
                    _projectionMatrix = _GeneratePerspectiveMatrix();
                    break;
                case CameraType.Orthographic:
                    _projectionMatrix = _GenerateOrthographicMatrix();
                    break;
            }

            _OnTransformChanged();
        }

        private Matrix3F _GeneratePerspectiveMatrix()
        {
            var mat = new Matrix3F();

            var tanHalfFovy = (float)Math.Tan(MathUtil.DegreesToRadians(_fieldOfView / 2));
            var deep = _farPlaneDistance - _nearPlaneDistance;

            mat.M11 = 1 / (_aspect * tanHalfFovy);
            mat.M22 = 1 / tanHalfFovy;
            mat.M33 = float.IsInfinity(_farPlaneDistance) ? -1 : -(_farPlaneDistance + _nearPlaneDistance) / deep;
            mat.M44 = 0;
            mat.M34 = -1;
            mat.OffsetZ = float.IsInfinity(_farPlaneDistance) ? -2 * _nearPlaneDistance : -2 * _farPlaneDistance * _nearPlaneDistance / deep;

            return mat;
        }

        private Matrix3F _GenerateOrthographicMatrix()
        {
            var mat = Matrix3F.Identity;
            var deep = _farPlaneDistance - _nearPlaneDistance;

            mat.M11 = 2 / _width;
            mat.M22 = 2 / _height;
            mat.M33 = float.IsInfinity(_farPlaneDistance) ? -0.00001f : -2 / deep;
            mat.OffsetZ = float.IsInfinity(_farPlaneDistance) ? -1 : -(_farPlaneDistance + _nearPlaneDistance) / deep;

            return mat;
        }

        private void _UpdateViewMatrix()
        {
            var zaxis = -_lookDirection;
            zaxis.Normalize();

            var xaxis = Vector3F.CrossProduct(_upDirection, zaxis);
            xaxis.Normalize();

            var yaxis = Vector3F.CrossProduct(zaxis, xaxis);

            var positionVec = (Vector3F)_position;
            var cx = -Vector3F.DotProduct(xaxis, positionVec);
            var cy = -Vector3F.DotProduct(yaxis, positionVec);
            var cz = -Vector3F.DotProduct(zaxis, positionVec);

            _viewMatrix = new Matrix3F(
                xaxis.X, yaxis.X, zaxis.X, 0,
                xaxis.Y, yaxis.Y, zaxis.Y, 0,
                xaxis.Z, yaxis.Z, zaxis.Z, 0,
                cx, cy, cz, 1);

            _OnTransformChanged();
        }

        private void _OnTransformChanged()
        {
            _totalTransform = _viewMatrix * _projectionMatrix;
            if (_totalTransform.HasInverse)
            {
                var totalTransformReverse = _totalTransform;
                totalTransformReverse.Invert();
                _totalTransformReverse = totalTransformReverse;
            }
            if (!_isLocked)
                PropertyChanged(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _viewPort = null;
        }
    }
}