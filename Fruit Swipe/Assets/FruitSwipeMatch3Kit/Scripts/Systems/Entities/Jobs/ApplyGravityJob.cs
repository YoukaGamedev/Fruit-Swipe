// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

namespace FruitSwipeMatch3Kit
{
    /// <summary>
    /// Internal job used by the gravity system.
    /// </summary>
    [BurstCompile]
    public struct ApplyGravityJob : IJob
    {
        public EntityCommandBuffer Ecb;
        public NativeArray<Entity> Tiles;

        public ComponentLookup<TilePosition> TilePosition;
        public ComponentLookup<LocalTransform> TranslationData;

        [ReadOnly] public int Width;
        [ReadOnly] public int Height;
        [ReadOnly] public float SpriteHeight;

        [ReadOnly] public ComponentLookup<HoleSlotData> HoleSlotData;
        [ReadOnly] public ComponentLookup<BlockerData> BlockerData;

        public void Execute()
        {
            bool gravityApplied = false;

            for (int i = 0; i < Width; i++)
            {
                for (int j = Height - 1; j >= 0; j--)
                {
                    int idx = i + j * Width;
                    Entity tile = Tiles[idx];

                    if (tile == Entity.Null ||
                        BlockerData.HasComponent(tile) ||
                        HoleSlotData.HasComponent(tile))
                        continue;

                    // Cari posisi bawah yang bisa diisi
                    int bottom = -1;
                    for (int k = j; k < Height; k++)
                    {
                        int idx2 = i + k * Width;
                        Entity bottomTile = Tiles[idx2];

                        if (bottomTile == Entity.Null && !HoleSlotData.HasComponent(bottomTile))
                            bottom = k;
                        else if (bottomTile != Entity.Null && BlockerData.HasComponent(bottomTile))
                            break;
                    }

                    if (bottom == -1 || tile == Entity.Null)
                        continue;

                    int numTilesToFall = bottom - j;
                    Tiles[idx + (numTilesToFall * Width)] = tile;

                    // Update posisi tile
                    var tilePos = TilePosition[tile];
                    Ecb.SetComponent(tile, new TilePosition
                    {
                        X = tilePos.X,
                        Y = tilePos.Y + numTilesToFall
                    });

                    // Update posisi world (transform)
                    var transform = TranslationData[tile];
                    Ecb.SetComponent(tile, new LocalTransform
                    {
                        Position = new float3(
                            transform.Position.x,
                            transform.Position.y - numTilesToFall * SpriteHeight,
                            transform.Position.z
                        ),
                        Rotation = transform.Rotation,
                        Scale = transform.Scale
                    });

                    Ecb.AddComponent<GravityTag>(tile);
                    gravityApplied = true;

                    Tiles[idx] = Entity.Null;
                }
            }

            // ECS 1.0 tidak bisa akses sistem lain langsung dari dalam job.
            // Maka, logika seperti `inputSystem.UnlockInput()` harus dipindahkan ke sistem luar job.
        }
    }
}
