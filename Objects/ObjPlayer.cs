using AI2D.Engine;

namespace AI2D.Objects
{
    public class ObjPlayer : ObjBase
    {
        private const string _imagePath = @"..\..\Assets\Graphics\Player\Default.png";

        public ObjPlayer(Core core)
            : base(core)
        {
            LoadResources(_imagePath, new System.Drawing.Size(32, 32));
        }
    }
}
