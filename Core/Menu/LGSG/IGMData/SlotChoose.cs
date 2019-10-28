﻿using Microsoft.Xna.Framework;
using System;

namespace OpenVIII
{
    namespace IGMData
    {
        public class SlotChoose : IGMData.Base
        {
            #region Properties

            public bool Save { get; protected set; } = false;
            public IGMDataItem.Box Slot1Main { get => (IGMDataItem.Box)ITEM[0, 0]; set => ITEM[0, 0] = value; }

            public IGMDataItem.Box Slot1Title { get => (IGMDataItem.Box)ITEM[0, 1]; set => ITEM[0, 1] = value; }

            public IGMDataItem.Box Slot2Main { get => (IGMDataItem.Box)ITEM[1, 0]; set => ITEM[1, 0] = value; }

            public IGMDataItem.Box Slot2Title { get => (IGMDataItem.Box)ITEM[1, 1]; set => ITEM[1, 1] = value; }

            #endregion Properties

            #region Methods

            public static SlotChoose Create(Rectangle pos)
            {
                SlotChoose r = new SlotChoose();
                r.Init(2, 2, new IGMDataItem.Empty(pos), 1, 2);
                return r;
            }

            public override bool Inputs() => base.Inputs();

            public override bool Inputs_CANCEL()
            {
                base.Inputs_CANCEL();
                if (!Save)
                    init_debugger_Audio.StopMusic();
                Menu.FadeIn();
                Module_main_menu_debug.State = Module_main_menu_debug.MainMenuStates.MainLobby;

                return true;
            }

            public override bool Inputs_OKAY()
            {
                base.Inputs_OKAY();
                IGM_LGSG.Mode mode = IGM_LGSG.Mode.Slot |
                        IGM_LGSG.Mode.Checking |
                        (Save ? IGM_LGSG.Mode.Save : IGM_LGSG.Mode.Nothing);

                if (CURSOR_SELECT == 0)
                    Menu.IGM_LGSG.SetMode(mode | IGM_LGSG.Mode.Slot1);
                else if (CURSOR_SELECT == 1)
                    Menu.IGM_LGSG.SetMode(mode | IGM_LGSG.Mode.Slot2);

                return true;
            }

            protected override void Init()
            {
                base.Init();
                Point offset = new Point(-8, -28);
                Point size = new Point(132, 60);
                Slot1Main = new IGMDataItem.Box { Data = Strings.Name.FF8, Pos = SIZE[0], Options = Box_Options.Buttom | Box_Options.Center };
                Slot2Main = new IGMDataItem.Box { Data = Strings.Name.FF8, Pos = SIZE[1], Options = Box_Options.Buttom | Box_Options.Center };
                Point p = SIZE[0].Location;
                p = p.Offset(offset);
                Slot1Title = new IGMDataItem.Box { Data = Strings.Name.Slot1, Pos = new Rectangle(p, size), Options = Box_Options.Middle | Box_Options.Center };
                p = SIZE[1].Location;
                p = p.Offset(offset);
                Slot2Title = new IGMDataItem.Box { Data = Strings.Name.Slot2, Pos = new Rectangle(p, size), Options = Box_Options.Middle | Box_Options.Center };
                Slot1Main.Draw(true);
                Slot2Main.Draw(true);
                CURSOR[0] = Slot1Main.Dims.Cursor;
                CURSOR[1] = Slot2Main.Dims.Cursor;
                Cursor_Status = Cursor_Status.Enabled;
            }

            protected override void InitShift(int i, int col, int row)
            {
                int SpaceBetween = 60;
                base.InitShift(i, col, row);
                switch (i)
                {
                    case 0:
                        SIZE[i].Y -= SpaceBetween / 2;
                        break;

                    case 1:
                    default:
                        SIZE[i].Y += row * SpaceBetween / 2;
                        break;
                }
            }

            protected override void ModeChangeEvent(object sender, Enum e)
            {
                base.ModeChangeEvent(sender, e);
                if (e.GetType() == typeof(IGM_LGSG.Mode))
                {
                    Save = e.HasFlag(IGM_LGSG.Mode.Save);
                    if (e.HasFlag(IGM_LGSG.Mode.Slot) && e.HasFlag(IGM_LGSG.Mode.Choose))
                        Show();
                    else
                        Hide();
                }
            }

            #endregion Methods
        }
    }
}