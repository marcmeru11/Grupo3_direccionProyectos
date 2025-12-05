using System;
using Programa1.layer;
using Programa1.Layer.Bridge;
using Programa1.Style.Background;
using Programa1.Style.Transition;

namespace Programa1.Style
{
    public class RenderStyle {
        private IBackground _background;
        private ITransition _transition;

        public IBackground Background {
            get => _background;
            set => _background = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ITransition Transition {
            get => _transition;
            set => _transition = value ?? throw new ArgumentNullException(nameof(value));
        }

        public RenderStyle(IBackground background, ITransition transition) {
            _background = background;
            _transition = transition;
        }

        public static RenderStyle Default() {
            return new RenderStyle(new MatrixBackground(), new DefaultTransition());
        }

        public void RenderBackground(RenderContext ctx) {
            _background.Render(ctx);
        }

        public void RenderTransition(RenderContext ctx) {
            _transition.Render(ctx);
        }

    }
    
}