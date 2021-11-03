using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YGeometry.DataStructure;
using YGeometry.IO;
using YRenderingSystem;
using YRenderingSystem._3D;

namespace YGeometry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _Init();

            Loaded += _OnLoaded;
        }

        GLPanel3D _glPanel3D;
        GLVisual3D _visual3D;
        GLMeshModel3D _meshModel;

        MeshData MeshData 
        {
            get { return _meshData; } 
            set 
            {
                if (_meshData != value)
                {
                    _meshData = value;
                    _UpdateMeshData();
                }
            }
        }
        MeshData _meshData;

        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= _OnLoaded;

            _glPanel3D.DisableAliased();
            _glPanel3D.Camera.PropertyChanged += Camera_PropertyChanged;
        }

        private void Camera_PropertyChanged(object sender, EventArgs e)
        {
        }

        private void _Init()
        {
            _glPanel3D = new GLPanel3D(Colors.Black);
            _glPanel3D.RotationSensitivity = 0.2f;
            GD_3D.Children.Add(_glPanel3D);

            _meshModel = new GLMeshModel3D() { Mode = GLPrimitiveMode.GL_TRIANGLES };

            _meshModel.AddMaterial(new DiffuseMaterial() { Color = Colors.White }, MaterialOption.Front);
            _meshModel.AddMaterial(new SpecularMaterial() { Color = Colors.White, SpecularPower = 128 }, MaterialOption.Front);
            _visual3D = new GLVisual3D();
            _visual3D.Model = _meshModel;
            _glPanel3D.AddVisual(_visual3D);
            _glPanel3D.AddLight(new AmbientLight(Colors.White, 0.2f));
            _glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(1, 0, 0), 0.15f, 0.1f));
            _glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(-1, 0, 0), 0.15f, 0.1f));
            _glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(0, 1, 0), 0.15f, 0.1f));
            _glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(0, -1, 0), 0.15f, 0.1f));
            _glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(0, 0, 1), 0.15f, 0.1f));
            _glPanel3D.AddLight(new DirectionLight(Colors.White, new Vector3F(0, 0, -1), 0.15f, 0.1f));
        }

        private void _OnImported(object sender, RoutedEventArgs e)
        {
            var meshData = Tests.TestImport();
            var mesh = MeshUtil.ConvertTo(meshData);
            var isClosed = mesh.IsClosed();
            //var mesh = Tests.TestCreate();
            MeshData = MeshUtil.ConvertTo(mesh);
        }

        private void _UpdateMeshData()
        {
            if (_meshData == null)
            {
                _meshModel.SetPoints(null);
                _meshModel.SetIndices(null);
                _meshModel.SetNormals(null);
            }
            else
            {
                _meshModel.SetPoints(_meshData.Vertices.Select(v => new Point3F((float)v.Position.X, (float)v.Position.Y, (float)v.Position.Z)));
                _meshModel.SetNormals(_meshData.Vertices.Select(v => new Vector3F((float)v.Normal?.X, (float)v.Normal?.Y, (float)v.Normal?.Z)));
                var indice = new List<int>();
                foreach (var face in _meshData.Faces)
                    indice.AddRange(face.Vertices);
                _meshModel.SetIndices(indice.Select(v => (uint)v));
            }
            _glPanel3D.FitView(_visual3D);
        }
    }
}