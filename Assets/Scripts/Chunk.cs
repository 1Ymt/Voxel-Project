﻿using System;
using UnityEngine;

public static class Chunk {

    public static void LoopThroughTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform) {
        for (int index = 0; index < chunkData.blocks.Length; index++) {

            var position = GetPostitionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }

    private static Vector3Int GetPostitionFromIndex(ChunkData chunkData, int index) {
        int x = index % chunkData.chunkSize;
        int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
        int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
        return new Vector3Int(x, y, z);
    }

    //in chunk coordinate system
    private static bool InRange(ChunkData chunkData, int axisCoordinate) {
        if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
            return false;

        return true;
    }

    //in chunk coordinate system
    private static bool InRangeHeight(ChunkData chunkData, int ycoordinate) {
        if (ycoordinate < 0 || ycoordinate >= chunkData.chunkHeight)
            return false;

        return true;
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates) {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z) {
        if (InRange(chunkData, x) && InRangeHeight(chunkData, y) && InRange(chunkData, z)) {

            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.blocks[index];
        }

        return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x, chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block) {
        if (InRange(chunkData, localPosition.x) && InRangeHeight(chunkData, localPosition.y) && InRange(chunkData, localPosition.z)) {

            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.blocks[index] = block;
        }else{
            throw new Exception("Need to ask World for appropiate chunk");
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z) {
        return x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z;
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos) {
        return new Vector3Int {
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z
        };
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData) {
        MeshData meshData = new MeshData(true);

        LoopThroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, x, y, z)]));


        return meshData;
    }

    public static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z) {
        Vector3Int pos = new Vector3Int
        {
            x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
            y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
            z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
        };
        return pos;
    }
}