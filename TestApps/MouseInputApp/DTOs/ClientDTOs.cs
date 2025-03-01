﻿using MPClientBase.DTO;
using System.Windows;
using System.Windows.Media;

namespace MouseInputApp.DTOs
{
    public class MouseMovePacket<T> : CPWithType<T, Purpose>
            where T : IDataPackTyped<T>, new()
    {
        public MouseMovePacket(Purpose purpose, T data) : base(purpose, data)
        {
        }
    }

    public class ColorData : IDataPackTyped<ColorData>
    {
        public Color Color { get; private set; }
        public ColorData()
        {

        }

        public ColorData(Color color)
        {
            Color = color;
        }

        public byte[] GetBytes() => new byte[] { Color.R, Color.G, Color.B };
        public ColorData ParseIn(byte[] data) => throw new NotImplementedException();
    }

    public class PositionData : IDataPackTyped<PositionData>
    {
        public Point Position { get; private set; }

        public PositionData()
        {
        }

        public PositionData(Point position)
        {
            Position = position;
        }

        public byte[] GetBytes()
        {
            var xbytes = BitConverter.GetBytes((short)Position.X);
            var ybytes = BitConverter.GetBytes((short)Position.Y);
            return new byte[] { xbytes[0], xbytes[1], ybytes[0], ybytes[1] };
        }

        public PositionData ParseIn(byte[] data) => throw new NotImplementedException();
    }

    public class PlayerPositions : IDataPackTyped<PlayerPositions>
    {
        public IList<PosAndColor> Positions { get; private set; }

        public PlayerPositions()
        {
            Positions = new List<PosAndColor>();
        }

        public byte[] GetBytes() => throw new NotImplementedException();

        public PlayerPositions ParseIn(byte[] packetData)
        {
            var count = packetData[0];

            for (int i = 0; i < count; i++)
            {
                var ofst = 1 + i * 7;
                var color = Color.FromRgb(packetData[ofst + 0], packetData[ofst + 1], packetData[ofst + 2]);
                var point = new Point(BitConverter.ToUInt16(packetData, ofst + 3), BitConverter.ToUInt16(packetData, ofst + 5));

                var entry = new PosAndColor() { Position = point, Color = color };
                Positions.Add(entry);
            }

            return this;
        }
    }

    public class PosAndColor
    {
        public Point Position { get; set; }
        public Color Color { get; set; }

    }
}
