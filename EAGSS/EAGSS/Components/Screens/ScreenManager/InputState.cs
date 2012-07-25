using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EAGSS
{
    public class InputState
    {
        public KeyboardState CurrentKeyboardState;
        public MouseState CurrentMouseState;

        public KeyboardState LastKeyboardState;
        public MouseState LastMouseState;

        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastMouseState = CurrentMouseState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// 鼠标是否在目标范围内
        /// </summary>
        /// <param name="rect">目标范围</param>
        public bool IsMouseInRectangle(Rectangle rect)
        {
            if (rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return true;

            return false;
        }

        /// <summary>
        /// 鼠标左键是否为按下状态
        /// </summary>
        public bool IsMouseLeftButtonPressed(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return false;

            return CurrentMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// 鼠标右键是否为按下状态
        /// </summary>
        public bool IsMouseRightButtonPressed(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return false;

            return CurrentMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// 鼠标左键是否刚刚按下
        /// </summary>
        public bool IsNewMouseLeftButtonPressed(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return false;

            return (LastMouseState.LeftButton == ButtonState.Released)
                   && (CurrentMouseState.LeftButton == ButtonState.Pressed);
        }

        /// <summary>
        /// 鼠标右键是否刚刚按下
        /// </summary>
        public bool IsNewMouseRightButtonPressed(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return false;

            return (LastMouseState.RightButton == ButtonState.Released)
                   && (CurrentMouseState.RightButton == ButtonState.Pressed);
        }

        /// <summary>
        /// 鼠标左键是否刚刚松开
        /// </summary>
        public bool IsNewMouseLeftButtonReleased(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return false;

            return (LastMouseState.LeftButton == ButtonState.Pressed)
                   && (CurrentMouseState.LeftButton == ButtonState.Released);
        }

        /// <summary>
        /// 鼠标右键是否刚刚松开
        /// </summary>
        public bool IsNewMouseRightButtonReleased(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return false;

            return (LastMouseState.RightButton == ButtonState.Pressed)
                   && (CurrentMouseState.RightButton == ButtonState.Released);
        }

        /// <summary>
        /// 得到鼠标位置的变化量
        /// </summary>
        public Point GetMouseLocationOffset(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return new Point();

            return new Point(CurrentMouseState.X - LastMouseState.X,
                             CurrentMouseState.Y - LastMouseState.Y);
        }

        /// <summary>
        /// 得到鼠标滚轮的变化格数
        /// </summary>
        public int GetMouseWheelOffset(Rectangle rect)
        {
            if (!rect.Contains(CurrentMouseState.X, CurrentMouseState.Y)) return 0;

            return (CurrentMouseState.ScrollWheelValue - LastMouseState.ScrollWheelValue) / 120;
        }

        /// <summary>
        /// 检测目标键是否刚被按下
        /// </summary>
        /// <param name="key">目标按键</param>
        public bool IsNewKeyPressed(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
        }
    }
}