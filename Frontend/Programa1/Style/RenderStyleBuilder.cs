using Programa1.Layer.Bridge;
using Programa1.Style.Background;
using Programa1.Style.Transition;

namespace Programa1.Style;

public class RenderStyleBuilder {
    
    private IBackground _background = new DefaultBackground();
    private ITransition _transition = new DefaultTransition();

    public void SetBackground(IBackground background) {
        this._background = background;
    }

    public void SetTransition(ITransition transition)
    {
        this._transition = transition;
    }

    public RenderStyle Build()
    {
        return new RenderStyle(_background, _transition);
    }
}
