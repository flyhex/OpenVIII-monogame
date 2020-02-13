﻿using Microsoft.Xna.Framework;

namespace OpenVIII.Battle
{
    public struct CharacterData
    {
        #region Fields

        public Debug_battleDat character, weapon;

        #endregion Fields

        #region Properties

        public Vector3 Location { get; internal set; }

        #endregion Properties
    };
}