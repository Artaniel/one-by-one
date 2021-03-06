using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnityEngine
{
    [Serializable]
    [CreateAssetMenu]
    public class CustomRuleTile : TileBase {
        
        public Sprite m_DefaultSprite;
		public Tile.ColliderType m_DefaultColliderType = Tile.ColliderType.Sprite;

        public TileBase m_Self {
            get { return m_OverrideSelf ? m_OverrideSelf : this; }
            set { m_OverrideSelf = value; }
        }
        private TileBase m_OverrideSelf;
        
        public bool firstFlag = true;
        public byte numberOfAccepted;
        public byte[] acceptedSetSize;

        public bool createdOnce = false;

        public bool firstFlag2 = true;
        public byte numberOfDeclined;
        public byte[] declinedSetSize;

        public const int basicRulesCount = 3;
        public const int gTilesMax = 7;
        public const int bTilesMax = 6;
        //public TileBase[] t1 = new TileBase[6], t2 = new TileBase[6], t3 = new TileBase[6], t4 = new TileBase[6], t5 = new TileBase[6], t6 = new TileBase[6], t7 = new TileBase[6];
        public TileBase[] t1, t2, t3, t4, t5, t6, t7;
        public TileBase[][] gTiles = new TileBase[gTilesMax][];
        //public TileBase[] n1 = new TileBase[6], n2 = new TileBase[6], n3 = new TileBase[6], n4 = new TileBase[6], n5 = new TileBase[6], n6 = new TileBase[6];
        public TileBase[] n1, n2, n3, n4, n5, n6;
        public TileBase[][] bTiles = new TileBase[bTilesMax][];

        [Serializable]
		public class TilingRule
		{
            public Neighbor[] m_Neighbors;
			public Sprite[] m_Sprites;
			public float m_AnimationSpeed;
			public float m_PerlinScale;
			public Transform m_RuleTransform;
			public OutputSprite m_Output;
			public Tile.ColliderType m_ColliderType;
			public Transform m_RandomTransform;

            public TilingRule()
			{
                m_Output = OutputSprite.Single;
				m_Neighbors = new Neighbor[8];
				m_Sprites = new Sprite[1];
				m_AnimationSpeed = 1f;
				m_PerlinScale = 0.5f;
				m_ColliderType = Tile.ColliderType.Sprite;

				for(int i=0; i<m_Neighbors.Length; i++)
					m_Neighbors[i] = Neighbor.DontCare;
			}

			public enum Transform { Fixed, Rotated, MirrorX, MirrorY }
			public enum Neighbor { DontCare, This, NotThis, These1, These2, These3, These4, These5, These6, These7, NotThese1, NotThese2, NotThese3, NotThese4, NotThese5, NotThese6 }
            public enum OutputSprite { Single, Random, Animation }
		}

		[HideInInspector] public List<TilingRule> m_TilingRules;

		public override void GetTileData(Vector3Int position, ITilemap tileMap, ref TileData tileData)
		{
			tileData.sprite = m_DefaultSprite;
			tileData.colliderType = m_DefaultColliderType;
			tileData.flags = TileFlags.LockTransform;
			tileData.transform = Matrix4x4.identity;
			
			foreach (TilingRule rule in m_TilingRules)
			{
				Matrix4x4 transform = Matrix4x4.identity;
				if (RuleMatches(rule, position, tileMap, ref transform))
				{
					switch (rule.m_Output)
					{
							case TilingRule.OutputSprite.Single:
							case TilingRule.OutputSprite.Animation:
								tileData.sprite = rule.m_Sprites[0];
							break;
							case TilingRule.OutputSprite.Random:
								int index = Mathf.Clamp(Mathf.FloorToInt(GetPerlinValue(position, rule.m_PerlinScale, 100000f) * rule.m_Sprites.Length), 0, rule.m_Sprites.Length - 1);
								tileData.sprite = rule.m_Sprites[index];
								if (rule.m_RandomTransform != TilingRule.Transform.Fixed)
									transform = ApplyRandomTransform(rule.m_RandomTransform, transform, rule.m_PerlinScale, position);
							break;
					}
					tileData.transform = transform;
					tileData.colliderType = rule.m_ColliderType;
					break;
				}
			}
		}

		private static float GetPerlinValue(Vector3Int position, float scale, float offset)
		{
			return Mathf.PerlinNoise((position.x + offset) * scale, (position.y + offset) * scale);
		}

		public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
		{
			foreach (TilingRule rule in m_TilingRules)
			{
				Matrix4x4 transform = Matrix4x4.identity;
				if (RuleMatches(rule, position, tilemap, ref transform) && rule.m_Output == TilingRule.OutputSprite.Animation)
				{
					tileAnimationData.animatedSprites = rule.m_Sprites;
					tileAnimationData.animationSpeed = rule.m_AnimationSpeed;
					return true;
				}
			}
			return false;
		}
		
		public override void RefreshTile(Vector3Int location, ITilemap tileMap)
		{
			if (m_TilingRules != null && m_TilingRules.Count > 0)
			{
				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						base.RefreshTile(location + new Vector3Int(x, y, 0), tileMap);
					}
				}
			}
			else
			{
				base.RefreshTile(location, tileMap);
			}
		}

		public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, ref Matrix4x4 transform)
		{
			// Check rule against rotations of 0, 90, 180, 270
			for (int angle = 0; angle <= (rule.m_RuleTransform == TilingRule.Transform.Rotated ? 270 : 0); angle += 90)
			{
				if (RuleMatches(rule, position, tilemap, angle))
				{
					transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);
					return true;
				}
			}

			// Check rule against x-axis mirror
			if ((rule.m_RuleTransform == TilingRule.Transform.MirrorX) && RuleMatches(rule, position, tilemap, true, false))
			{
				transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
				return true;
			}

			// Check rule against y-axis mirror
			if ((rule.m_RuleTransform == TilingRule.Transform.MirrorY) && RuleMatches(rule, position, tilemap, false, true))
			{
				transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
				return true;
			}

			return false;
		}

		private static Matrix4x4 ApplyRandomTransform(TilingRule.Transform type, Matrix4x4 original, float perlinScale, Vector3Int position)
		{
			float perlin = GetPerlinValue(position, perlinScale, 200000f);
			switch (type)
			{
				case TilingRule.Transform.MirrorX:
					return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(perlin < 0.5 ? 1f : -1f, 1f, 1f));
				case TilingRule.Transform.MirrorY:
					return original * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, perlin < 0.5 ? 1f : -1f, 1f));
				case TilingRule.Transform.Rotated:
					int angle = Mathf.Clamp(Mathf.FloorToInt(perlin * 4), 0, 3) * 90;
					return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -angle), Vector3.one);		
			}
			return original;
		}

		public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, int angle)
		{
            ReinitTilesArray();

            for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					if (x != 0 || y != 0)
					{
						Vector3Int offset = new Vector3Int(x, y, 0);
						Vector3Int rotated = GetRotatedPos(offset, angle);
						int index = GetIndexOfOffset(rotated);
						TileBase tile = tilemap.GetTile(position + offset);
                        if (!AcceptTile(rule, tile, rule.m_Neighbors[index])) return false;
					}
				}
				
			}
			return true;
		}

        private bool AcceptTile(TilingRule rule, TileBase tile, TilingRule.Neighbor RULE) {
            TileBase[] tilesInRule;
            int ruleNumber = (int)RULE;
            

            if (RULE == TilingRule.Neighbor.DontCare)
                return true;
            else if (RULE == TilingRule.Neighbor.This && tile == m_Self || RULE == TilingRule.Neighbor.NotThis && tile != m_Self)
                return true;
            else if (ruleNumber >= basicRulesCount && ruleNumber < basicRulesCount + gTilesMax) {
                tilesInRule = gTiles[ruleNumber - basicRulesCount];
                foreach (TileBase t in tilesInRule) {
                    if (t == tile) return true;
                }
                return false;
            }
            else if (ruleNumber >= basicRulesCount + gTilesMax) {
                tilesInRule = bTiles[ruleNumber - gTilesMax - basicRulesCount];
                foreach (TileBase t in tilesInRule) {
                    if (t == tile) return false;
                }
                return true;
            }
            return false;
        }


        public void ReinitTilesArray()
        {
            if (acceptedSetSize == null || acceptedSetSize.Length == 0)
            {
                acceptedSetSize = new byte[7];
                declinedSetSize = new byte[6];
            }
            if (gTiles == null || gTiles[0] == null)
            {
                gTiles = new TileBase[][] { t1, t2, t3, t4, t5, t6, t7 };
                bTiles = new TileBase[][] { n1, n2, n3, n4, n5, n6 };
            }
        }


		public bool RuleMatches(TilingRule rule, Vector3Int position, ITilemap tilemap, bool mirrorX, bool mirrorY)
		{
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
                    if (x == 0 && y == 0) continue;
					
					Vector3Int offset = new Vector3Int(x, y, 0);
					Vector3Int mirrored = GetMirroredPos(offset, mirrorX, mirrorY);
					int index = GetIndexOfOffset(mirrored);
					TileBase tile = tilemap.GetTile(position + offset);
					if (!AcceptTile(rule, tile, rule.m_Neighbors[index])) return false;
				}
			}
			
			return true;
		}

		private int GetIndexOfOffset(Vector3Int offset)
		{
			int result = offset.x + 1 + (-offset.y + 1) * 3;
			if (result >= 4)
				result--;
			return result;
		}

		public Vector3Int GetRotatedPos(Vector3Int original, int rotation)
		{
			switch (rotation)
			{
				case 0:
					return original;
				case 90:
					return new Vector3Int(-original.y, original.x, original.z);
				case 180:
					return new Vector3Int(-original.x, -original.y, original.z);
				case 270:
					return new Vector3Int(original.y, -original.x, original.z);
			}
			return original;
		}

		public Vector3Int GetMirroredPos(Vector3Int original, bool mirrorX, bool mirrorY)
		{
			return new Vector3Int(original.x * (mirrorX ? -1 : 1), original.y * (mirrorY ? -1 : 1), original.z);
		}
    }
}
