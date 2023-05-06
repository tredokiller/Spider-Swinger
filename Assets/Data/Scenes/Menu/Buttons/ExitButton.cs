using Data.Common.Button;
using UnityEngine.Device;


namespace Data.Scenes.Menu.Buttons
{
    public class ExitButton : ButtonBase
    {
        public void Exit()
        {
            Application.Quit();
        }
    }
}
