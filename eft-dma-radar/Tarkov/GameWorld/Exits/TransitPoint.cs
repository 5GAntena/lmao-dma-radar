﻿using eft_dma_radar.Tarkov.EFTPlayer;
using eft_dma_radar.UI.ESP;
using eft_dma_radar.UI.Misc;
using eft_dma_radar.UI.Radar;
using eft_dma_shared.Common.ESP;
using eft_dma_shared.Common.Maps;
using eft_dma_shared.Common.Misc;
using eft_dma_shared.Common.Misc.Data;
using eft_dma_shared.Common.Players;
using eft_dma_shared.Common.Unity;

namespace eft_dma_radar.Tarkov.GameWorld.Exits
{
    public sealed class TransitPoint : IExitPoint, IWorldEntity, IMapEntity, IMouseoverEntity, IESPEntity
    {
        public static implicit operator ulong(TransitPoint x) => x._addr;
        private static readonly uint[] _transformInternalChain =
{
            ObjectClass.MonoBehaviourOffset, MonoBehaviour.GameObjectOffset, GameObject.ComponentsOffset, 0x8
        };

        public TransitPoint(ulong baseAddr)
        {
            _addr = baseAddr;

            var parameters = Memory.ReadPtr(baseAddr + Offsets.TransitPoint.parameters, false);
            var locationPtr = Memory.ReadPtr(parameters + Offsets.TransitParameters.location, false);
            var location = Memory.ReadUnityString(locationPtr, 64, false);
            if (GameData.MapNames.TryGetValue(location, out string destinationMapName))
            {
                Name = $"Transit to {destinationMapName}";
            }
            else
            {
                Name = "Transit";
            }
            var transformInternal = Memory.ReadPtrChain(baseAddr, _transformInternalChain, false);
            try
            {
                _position = new UnityTransform(transformInternal, false).UpdatePosition();
            }
            catch (ArgumentOutOfRangeException) // Fixes a bug on interchange
            {
                _position = new(0, -100, 0);
            }
        }

        private readonly ulong _addr;
        public string Name { get; }

        #region Interfaces

        private Vector3 _position;
        public ref Vector3 Position => ref _position;
        public Vector2 MouseoverPosition { get; set; }

        public void Draw(SKCanvas canvas, LoneMapParams mapParams, ILocalPlayer localPlayer)
        {
            var heightDiff = Position.Y - localPlayer.Position.Y;
            var paint = GetPaint();
            var point = Position.ToMapPos(mapParams.Map).ToZoomedPos(mapParams);
            MouseoverPosition = new Vector2(point.X, point.Y);
            SKPaints.ShapeOutline.StrokeWidth = 2f;
            if (heightDiff > 1.85f) // exfil is above player
            {
                using var path = point.GetUpArrow(6.5f);
                canvas.DrawPath(path, SKPaints.ShapeOutline);
                canvas.DrawPath(path, paint);
            }
            else if (heightDiff < -1.85f) // exfil is below player
            {
                using var path = point.GetDownArrow(6.5f);
                canvas.DrawPath(path, SKPaints.ShapeOutline);
                canvas.DrawPath(path, paint);
            }
            else // exfil is level with player
            {
                float size = 4.75f * MainForm.UIScale;
                canvas.DrawCircle(point, size, SKPaints.ShapeOutline);
                canvas.DrawCircle(point, size, paint);
            }
        }

        private static SKPaint GetPaint()
        {
            var localPlayer = Memory.LocalPlayer;
            if (!(localPlayer?.IsPmc ?? false))
                return SKPaints.PaintExfilInactive;
            return SKPaints.PaintExfilTransit;
        }

        public void DrawMouseover(SKCanvas canvas, LoneMapParams mapParams, LocalPlayer localPlayer)
        {
            // Save the current canvas state
            canvas.Save();

            // Get the quest location's position on the map
            var TransitPosition = Position.ToMapPos(mapParams.Map).ToZoomedPos(mapParams);

            // Apply a rotation transformation to the canvas
            float rotation = MainForm.Window._rotationDegrees;
            canvas.RotateDegrees(rotation, TransitPosition.X, TransitPosition.Y);

            // Adjust text orientation for 90° and 270° rotations
            if (rotation == 90 || rotation == 270)
            {
                canvas.RotateDegrees(180, TransitPosition.X, TransitPosition.Y);
            }

            // Draw the mouseover text
            List<string> lines = new(1)
            {
                Name
            };
            TransitPosition.DrawMouseoverText(canvas, lines);

            // Restore the canvas state
            canvas.Restore();
        }

        public void DrawESP(SKCanvas canvas, LocalPlayer localPlayer)
        {
            if (!localPlayer.IsPmc)
                return;
            if (!CameraManagerBase.WorldToScreen(ref _position, out var scrPos))
                return;
            scrPos.DrawESPText(canvas, this, localPlayer, ESP.Config.ShowDistances, SKPaints.TextExfilESP, Name);
        }

        #endregion

    }
}
