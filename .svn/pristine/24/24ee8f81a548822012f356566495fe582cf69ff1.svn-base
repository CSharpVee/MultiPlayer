using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace MultiPlayer.UIShaders
{
    public class GrayscaleFX : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleFX), 0);

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public GrayscaleFX()
        {
            var pixelShader = new PixelShader();

            var shPath = "pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/UIShaders/PrecompiledShaders/GrayscaleShader.ps";
            pixelShader.UriSource = new Uri(shPath, UriKind.RelativeOrAbsolute);
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
        }
    }
}
