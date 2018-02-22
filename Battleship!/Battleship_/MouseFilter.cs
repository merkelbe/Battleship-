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

    // Goal of this class is to support click / double-click recognition for any mouse input (left-click, right-click, middle-click, etc.)
    internal abstract class ButtonInfo
    {
        InputType singleClick;
        InputType doubleClick;

        bool wasDown; // Tracks button state from last loop
        bool isDown; // Button state from this loop

        Point startingCoordinates;
        Point endingCoordinates;

        bool isFirstClick;
        bool isDoubleClick;

        const int doubleClickWaitTime = 2; // Represents the amount of time to wait for the user to double click before firing off the single click event.
        int doubleClickWaitTimer; 

        internal ButtonInfo (InputType _singleClick, InputType _doubleClick)
        {
            singleClick = _singleClick;
            doubleClick = _doubleClick;

            wasDown = false;
            isDown = false;

            isFirstClick = true;
            isDoubleClick = false;
            
            doubleClickWaitTimer = 0;
            
            startingCoordinates = new Point();
            endingCoordinates = new Point();
        }

        internal abstract bool isButtonDown(MouseState _mouseState); // Should be specific to the button we're tracking (left mouse vs right mouse, etc.)

        internal void Update(MouseState _mouseState, ref List<ClickInfo> _actionQueue)
        {
            // Ticks down double click wait time, if applicable
            doubleClickWaitTimer = Math.Max(doubleClickWaitTimer - 1, 0);

            isDown = isButtonDown(_mouseState);
            bool click = !isDown && wasDown;
            if (click)
            {
                if (isFirstClick)
                {
                    doubleClickWaitTimer = doubleClickWaitTime + 1; // Questionable hack here.  Sets timer to N+1 and then checks double click stuff when timer reaches 1.
                    isFirstClick = false;
                }
                else
                {
                    isDoubleClick = true;
                    doubleClickWaitTimer = 1; // Short-cuts it so the double click is immediately registered and the user doesn't have to wait for the timer
                }
            }

            if(doubleClickWaitTimer == 1)
            {
                // Register click
                _actionQueue.Add(new ClickInfo(isDoubleClick ? doubleClick: singleClick, new Point(_mouseState.X, _mouseState.Y), new Point(_mouseState.X, _mouseState.Y)));

                // Reset data
                isFirstClick = true;
                isDoubleClick = false;
            }

            wasDown = isDown;
        }
    }



    //TODO Set private.  Made public for debugging.
    //internal abstract class ButtonInfo
    //{
    //    InputType inputTypeSingle;
    //    InputType inputTypeDouble;
    //    bool lastUpdateIsDown;
    //    bool currentIsDown;
    //    int stateChangeCount;

    //    Point startingCoordinates;
    //    Point endingCoordinates;

    //    const int waitTimerInTicks = 10;
    //    private int currentTimer;
        
    //    internal ButtonInfo(InputType _inputTypeSingle, InputType _inputTypeDouble)
    //    {
    //        inputTypeSingle = _inputTypeSingle;
    //        inputTypeDouble = _inputTypeDouble;
    //        startingCoordinates = new Point();
    //        endingCoordinates = new Point();
    //    }

    //    internal abstract bool isButtonDown(MouseState _mouseState);

    //    internal void Update(MouseState _mouseState, ref List<ClickInfo> _actionQueue)
    //    {
    //        currentIsDown = isButtonDown(_mouseState);

    //        //Start of tracking condition
    //        if (currentIsDown && !lastUpdateIsDown && stateChangeCount == 0)
    //        {
    //            stateChangeCount = 1;
    //            currentTimer = waitTimerInTicks;
    //            startingCoordinates = new Point(_mouseState.X, _mouseState.Y);
    //        }
    //        //Tracks changes in up/down states
    //        else if (currentIsDown != lastUpdateIsDown)
    //        {
    //            ++stateChangeCount;
    //            currentTimer += 1;
    //            //Ending coords of single click
    //            if (stateChangeCount == 2)
    //            {
    //                endingCoordinates = new Point(_mouseState.X, _mouseState.Y);
    //            }
    //        }
    //        lastUpdateIsDown = currentIsDown;
    //        --currentTimer;
    //        //End of tracking condition
    //        if (currentTimer <= 0 || stateChangeCount >= 4)
    //        {
    //            if (stateChangeCount >= 4)
    //            {
    //                //Ending coords of double click. Overrides single click ending coords.
    //                endingCoordinates = new Point(_mouseState.X, _mouseState.Y);

    //                _actionQueue.Add(new ClickInfo(inputTypeDouble, startingCoordinates, endingCoordinates));
    //            }
    //            else if (stateChangeCount >= 2)
    //            {
    //                _actionQueue.Add(new ClickInfo(inputTypeSingle, startingCoordinates, endingCoordinates));
    //            }
    //            stateChangeCount = 0;
    //            currentTimer = 0;
    //        }
    //    }
    //}

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
