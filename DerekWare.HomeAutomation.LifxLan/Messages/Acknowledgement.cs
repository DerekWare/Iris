﻿namespace DerekWare.HomeAutomation.Lifx.Lan.Messages
{
    class Acknowledgement : Response
    {
        public new const ushort MessageType = 45;

        #region Conversion

        public override bool Parse()
        {
            return true;
        }

        #endregion
    }
}
