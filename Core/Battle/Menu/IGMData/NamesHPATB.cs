﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenVIII.IGMData
{
    public class NamesHPATB : IGMData.Base
    {
        #region Fields

        private const int ATBWidth = 150;

        private static Texture2D dot;
        private static object locker = new object();
        private bool EventAdded = false;

        #endregion Fields

        #region Destructors

        ~NamesHPATB()
        {
            if (EventAdded)
                RemoveModeChangeEvent(ref Damageable.BattleModeChangeEventHandler);
        }

        #endregion Destructors

        #region Enums

        private enum DepthID : byte
        {
            Name,
            HP,
            ATBCharging,
            ATBCharged,
            ATBBorder
        }

        #endregion Enums

        #region Methods

        public static NamesHPATB Create(Rectangle pos, Damageable damageable) => Create<NamesHPATB>(1, 5, new IGMDataItem.Empty(pos), 1, 3, damageable);

        public static Texture2D ThreadUnsafeOperations()
        {
            lock (locker)
            {
                if (dot == null)
                {
                    //if (Memory.IsMainThread)
                    //{
                    Texture2D localdot = new Texture2D(Memory.graphics.GraphicsDevice, 4, 4);
                    Color[] tmp = new Color[localdot.Height * localdot.Width];
                    for (int i = 0; i < tmp.Length; i++)
                        tmp[i] = Color.White;
                    localdot.SetData(tmp);
                    dot = localdot;
                    IGMDataItem.Gradient.ATB.ThreadUnsafeOperations(ATBWidth);
                    //}
                    //else throw new Exception("Must be in main thread!");
                }
                return dot;
            }
        }

        public override void Refresh(Damageable damageable)
        {
            if (EventAdded && damageable != Damageable)
            {
                EventAdded = false;
                if (Damageable != null)
                    RemoveModeChangeEvent(ref Damageable.BattleModeChangeEventHandler);
            }
            base.Refresh(damageable);
        }

        public override void Refresh()
        {
            if (Memory.State?.Characters != null && Damageable != null && Damageable.GetCharacterData(out Saves.CharacterData c))
            {
                List<KeyValuePair<int, Characters>> party = GetParty();
                byte pos = GetCharPos(party);
                if (pos == 0xFF) return;
                Rectangle atbbarpos = new Rectangle(SIZE[pos].X + 230, SIZE[pos].Y + 12, ATBWidth, 15);
                ((IGMDataItem.Gradient.ATB)ITEM[0, (int)DepthID.ATBCharging]).Pos = atbbarpos;
                ((IGMDataItem.Texture)ITEM[0, (byte)DepthID.ATBCharged]).Pos = atbbarpos;
                ((IGMDataItem.Icon)ITEM[0, (byte)DepthID.ATBBorder]).Pos = atbbarpos;
                ((IGMDataItem.Text)ITEM[0, (byte)DepthID.Name]).Data = c.Name;
                ((IGMDataItem.Text)ITEM[0, (byte)DepthID.Name]).Pos = new Rectangle(SIZE[pos].X, SIZE[pos].Y, 0, 0);
                ((IGMDataItem.Integer)ITEM[0, (byte)DepthID.HP]).Pos = new Rectangle(SIZE[pos].X + 128, SIZE[pos].Y, 0, 0);
                if (EventAdded == false)
                {
                    EventAdded = true;
                    AddModeChangeEvent(ref Damageable.BattleModeChangeEventHandler);
                }
                bool blink = false;
                bool charging = false;
                if (Damageable.GetBattleMode().Equals(Damageable.BattleMode.YourTurn))
                {
                    ((IGMDataItem.Texture)ITEM[0, (int)DepthID.ATBCharged]).Color = Color.LightYellow * .8f;
                    blink = true;
                }
                else if (Damageable.GetBattleMode().Equals(Damageable.BattleMode.ATB_Charged))
                {
                    ((IGMDataItem.Texture)ITEM[0, (int)DepthID.ATBCharged]).Color = Color.Yellow * .8f;
                }
                else if (Damageable.GetBattleMode().Equals(Damageable.BattleMode.ATB_Charging))
                {
                    charging = true;
                    ((IGMDataItem.Gradient.ATB)ITEM[0, (int)DepthID.ATBCharging]).Refresh(Damageable);
                }
                    ((IGMDataItem.Texture)ITEM[0, (int)DepthID.ATBCharged]).Blink = blink;
                if (charging)
                {
                    ITEM[0, (int)DepthID.ATBCharged].Hide();
                    ITEM[0, (int)DepthID.ATBCharging].Show();
                }
                else
                {
                    ITEM[0, (int)DepthID.ATBCharging].Hide();
                    ITEM[0, (int)DepthID.ATBCharged].Show();
                }
                    ((IGMDataItem.Text)ITEM[0, (byte)DepthID.Name]).Blink = blink;
                ((IGMDataItem.Integer)ITEM[0, (byte)DepthID.HP]).Blink = blink;

                base.Refresh();
            }
        }

        public override bool Update()
        {
            if (ITEM[0, 2].GetType() == typeof(IGMDataItem.Gradient.ATB))
            {
                IGMDataItem.Gradient.ATB hg = (IGMDataItem.Gradient.ATB)ITEM[0, 2];
            }
            if (Damageable != null && Damageable.GetCharacterData(out Saves.CharacterData c))
            {
                int HP = c.CurrentHP();
                int CriticalHP = c.CriticalHP();
                Font.ColorID colorid = Font.ColorID.White;
                if (HP < CriticalHP)
                {
                    colorid = Font.ColorID.Yellow;
                }
                if (HP <= 0)
                {
                    colorid = Font.ColorID.Red;
                }
                ((IGMDataItem.Text)ITEM[0, (byte)DepthID.Name]).FontColor = colorid;
                ((IGMDataItem.Integer)ITEM[0, (byte)DepthID.HP]).Data = HP;
                ((IGMDataItem.Integer)ITEM[0, (byte)DepthID.HP]).FontColor = colorid;
            }
            return base.Update();
        }

        protected override void Init()
        {
            base.Init();
            ThreadUnsafeOperations();

            // TODO: make a font render that can draw right to left from a point. For Right aligning the names.
            Rectangle atbbarpos = new Rectangle(SIZE[0].X + 230, SIZE[0].Y + 12, ATBWidth, 15);
            ITEM[0, (byte)DepthID.Name] = new IGMDataItem.Text { };
            ITEM[0, (byte)DepthID.HP] = new IGMDataItem.Integer { Spaces = 4, NumType = Icons.NumType.Num_8x16_1 };
            ITEM[0, (byte)DepthID.ATBBorder] = new IGMDataItem.Icon { Data = Icons.ID.Size_08x64_Bar, Palette = 0 };
            ITEM[0, (byte)DepthID.ATBCharged] = new IGMDataItem.Texture { Data = dot, Color = Color.LightYellow * .8f, Faded_Color = new Color(125, 125, 0, 255) * .8f };
            ITEM[0, (byte)DepthID.ATBCharged].Hide();
            ITEM[0, (int)DepthID.ATBCharging] = IGMDataItem.Gradient.ATB.Create(atbbarpos);
            ((IGMDataItem.Gradient.ATB)ITEM[0, (byte)DepthID.ATBCharging]).Color = Color.Orange * .8f;
            ((IGMDataItem.Gradient.ATB)ITEM[0, (byte)DepthID.ATBCharging]).Faded_Color = Color.Orange * .8f;
            ((IGMDataItem.Gradient.ATB)ITEM[0, (byte)DepthID.ATBCharging]).Refresh(Damageable);
        }

        protected override void ModeChangeEvent(object sender, Enum e)
        {
            base.ModeChangeEvent(sender, e);
            if (!e.Equals(Damageable.BattleMode.EndTurn)) //because endturn triggers BattleMenu refresh.
                Refresh();
        }

        private static List<KeyValuePair<int, Characters>> GetParty()
        {
            if (Memory.State != null && Memory.State.Characters != null)
                return Memory.State.Party.Select((element, index) => new { element, index }).ToDictionary(m => m.index, m => m.element).Where(m => !m.Value.Equals(Characters.Blank)).ToList();
            return null;
        }

        private byte GetCharPos() => GetCharPos(GetParty());

        private byte GetCharPos(List<KeyValuePair<int, Characters>> party)
        {
            int i = -1;
            if (party != null && (i = party.FindIndex(x => Damageable.GetCharacterData(out Saves.CharacterData c) && x.Value == c.ID)) > -1)
                return checked((byte)i);
            return 0xFF;
        }

        #endregion Methods
    }
}