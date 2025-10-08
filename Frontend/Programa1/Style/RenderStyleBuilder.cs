using Programa1.Layer.Bridge;
using Programa1.Style.Background;

namespace Programa1.Style;

public class RenderStyleBuilder {
    
    private IBackground _background = new DefaultBackground();

    public void SetBackground(IBackground background) {
        this._background = background;
    }
    
    public RenderStyle Build() {
        return new RenderStyle(_background);
    }

}