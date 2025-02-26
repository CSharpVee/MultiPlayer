using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System;

namespace MultiPlayer.UIShaders
{
    public class DiffuseColorFX : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(DiffuseColorFX), 0);
        public static readonly DependencyProperty DiffuseColorProperty = DependencyProperty.Register(nameof(DiffuseColor), typeof(Color), typeof(DiffuseColorFX), new UIPropertyMetadata(Colors.Transparent, PixelShaderConstantCallback(0)));

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public Color DiffuseColor
        {
            get { return (Color)GetValue(DiffuseColorProperty); }
            set { SetValue(DiffuseColorProperty, value); }
        }

        public DiffuseColorFX()
        {
            var pixelShader = new PixelShader();

            var shPath = "pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/UIShaders/PrecompiledShaders/DiffuseShader.ps";
            pixelShader.UriSource = new Uri(shPath, UriKind.RelativeOrAbsolute);
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DiffuseColorProperty);
        }
    }
}
