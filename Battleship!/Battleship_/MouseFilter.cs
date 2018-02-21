using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Battleship_
{
    internal enum InputType { Nothing, LeftClick, LeftDoubleClick, RightClick, RightDoubleClick };

    class MouseFilter
    {
        //Mouse Filter class adds to queue when it registers mouse commands.  Inputs are accessed and removed by parent game class.
        internal List<ClickInfo> ActionQueue;

        LeftClickInfo leftButtonInfo;
        RightClickInfo rightButtonInfo;

        internal MouseFilter()
        {
            leftButtonInfo = new LeftClickInfo(InputType.LeftClick, InputType.LeftDoubleClick);
            rightButtonInfo = new RightClickInfo(InputType.RightClick, InputType.RightDoubleClick);
        }

        internal void Update(MouseState _mouseState, ref List<ClickInfo> _actionQueue)
        {
            leftButtonInfo.Update(_mouseState, ref _actionQueue);
            rightButtonInfo.Update(_mouseState, ref _actionQueue);
        }
    }

    //TODO Set private.  Made public for debugging.
    internal abstract class ButtonInfo
    {
        InputType inputTypeSingle;
        InputType inputTypeDouble;
        bool lastUpdateIsDown;
        bool currentIsDown;
        int stateChangeCount;

        Point startingCoordinates;
        Point endingCoordinates;

        const int resetTimerInTicks = 10;
        private int currentTimer;


        internal ButtonInfo(InputType _inputTypeSingle, InputType _inputTypeDouble)
        {
            inputTypeSingle = _inputTypeSingle;
            inputTypeDouble = _inputTypeDouble;
            startingCoordinates = new Point();
            endingCoordinates = new Point();
        }

        internal abstract bool isButtonDown(MouseState _mouseState);

        internal void Update(MouseState _mouseState, ref List<ClickInfo> _actionQueue)
        {
            currentIsDown = isButtonDown(_mouseState);

            //Start of tracking condition
            if (currentIsDown && !lastUpdateIsDown && stateChangeCount == 0)
            {
                stateChangeCount = 1;
                currentTimer = resetTimerInTicks;
                startingCoordinates = new Point(_mouseState.X, _mouseState.Y);
            }
            //Tracks changes in up/down states
            else if (currentIsDown != lastUpdateIsDown)
            {
                ++stateChangeCount;
                currentTimer += 1;
                //Ending coords of single click
                if (stateChangeCount == 2)
                {
                    endingCoordinates = new Point(_mouseState.X, _mouseState.Y);
                }
            }
            lastUpdateIsDown = currentIsDown;
            --currentTimer;
            //End of tracking condition
            if (currentTimer <= 0 || stateChangeCount >= 4)
            {
                if (stateChangeCount >= 4)
                {
                    //Ending coords of double click. Overrides single click ending coords.
                    endingCoordinates = new Point(_mouseState.X, _mouseState.Y);

                    _actionQueue.Add(new ClickInfo(inputTypeDouble, startingCoordinates, endingCoordinates));
                }
                else if (stateChangeCount >= 2)
                {
                    _actionQueue.Add(new ClickInfo(inputTypeSingle, startingCoordinates, endingCoordinates));
                }
                stateChangeCount = 0;
                currentTimer = 0;
            }
        }
    }

    class LeftClickInfo : ButtonInfo
    {
        public LeftClickInfo(InputType _inputTypeSingle, InputType _inputTypeDouble) : base(_inputTypeSingle, _inputTypeDouble)
        {
        }

        internal override bool isButtonDown(MouseState _mouseState)
        {
            return _mouseState.LeftButton == ButtonState.Pressed;
        }
    }

    class RightClickInfo : ButtonInfo
    {
        public RightClickInfo(InputType _inputTypeSingle, InputType _inputTypeDouble) : base(_inputTypeSingle, _inputTypeDouble)
        {
        }

        internal override bool isButtonDown(MouseState _mouseState)
        {
            return _mouseState.RightButton == ButtonState.Pressed;
        }
    }

    class ClickInfo
    {
        internal InputType ClickType;
        internal Point StartingCoordinates;
        internal Point EndingCoordinates;

        internal ClickInfo(InputType _clickType, Point _startingCoordinates, Point _endingCoordinates)
        {
            ClickType = _clickType;
            StartingCoordinates = _startingCoordinates;
            EndingCoordinates = _endingCoordinates;
        }
    }
}
