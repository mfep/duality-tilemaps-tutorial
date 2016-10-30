using Duality;
using Duality.Components;
using Duality.Editor;
using Duality.Input;
using Duality.Plugins.Tilemaps;

namespace Duality_
{
    [EditorHintCategory ("Tilemaps Tutorial")]
    public class ChangeTilemapCmp : Component, ICmpUpdatable
    {
        private Tilemap         TilemapInScene         => this.GameObj.ParentScene.FindComponent<Tilemap> ();
        private TilemapRenderer TilemapRendererInScene => this.GameObj.ParentScene.FindComponent<TilemapRenderer>();
        private Transform       MainCameraTransform    => this.GameObj.ParentScene.FindComponent<Camera> ()?.GameObj.Transform;

        void ICmpUpdatable.OnUpdate ()
        {
            if (DualityApp.Mouse.ButtonHit (MouseButton.Left))
                ChangeTilemap (GetWorldCoordOfMouse ());
        }

        private Vector2 GetWorldCoordOfMouse ()
        {
            Vector2 mouseScreenPos = DualityApp.Mouse.Pos;
            Vector2 screenCenter   = DualityApp.TargetResolution / 2;
            Vector3 cameraPos      = MainCameraTransform?.Pos ?? Vector3.Zero;

            return mouseScreenPos - screenCenter + cameraPos.Xy;
        }

        private void ChangeTilemap (Vector2 worldPos)
        {
            Tilemap tilemap = TilemapInScene;
            TilemapRenderer tilemapRenderer = TilemapRendererInScene;
            if (tilemap == null || tilemapRenderer == null) {
                Log.Game.WriteError("There are no tilemaps in the current scene!");
                return;
            }

            Vector2 localPos = worldPos - tilemapRenderer.GameObj.Transform.Pos.Xy;
            Point2 tilePos = tilemapRenderer.GetTileAtLocalPos (localPos, TilePickMode.Reject);
            if (tilePos.X < 0 || tilePos.Y < 0)
                return;

            Tile clickedTile = tilemap.Tiles[tilePos.X, tilePos.Y];
            int newTileIndex = clickedTile.BaseIndex == 0 ? 1 : 0;
            clickedTile.BaseIndex = newTileIndex;
            tilemap.SetTile(tilePos.X, tilePos.Y, clickedTile);
        }
    }
}