using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Sprites;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    [CustomEditor(typeof(CustomRuleTile))]
	[CanEditMultipleObjects]
	internal class CustomRuleTileEditor : Editor
	{
        // Standard
        static GUIStyle grey = new GUIStyle();
        private const string s_XIconString = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABoSURBVDhPnY3BDcAgDAOZhS14dP1O0x2C/LBEgiNSHvfwyZabmV0jZRUpq2zi6f0DJwdcQOEdwwDLypF0zHLMa9+NQRxkQ+ACOT2STVw/q8eY1346ZlE54sYAhVhSDrjwFymrSFnD2gTZpls2OvFUHAAAAABJRU5ErkJggg==";
		private const string s_Arrow0 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPzZExDoQwDATzE4oU4QXXcgUFj+YxtETwgpMwXuFcwMFSRMVKKwzZcWzhiMg91jtg34XIntkre5EaT7yjjhI9pOD5Mw5k2X/DdUwFr3cQ7Pu23E/BiwXyWSOxrNqx+ewnsayam5OLBtbOGPUM/r93YZL4/dhpR/amwByGFBz170gNChA6w5bQQMqramBTgJ+Z3A58WuWejPCaHQAAAABJRU5ErkJggg==";
		private const string s_Arrow1 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPxYzBDYAgEATpxYcd+PVr0fZ2siZrjmMhFz6STIiDs8XMlpEyi5RkO/d66TcgJUB43JfNBqRkSEYDnYjhbKD5GIUkDqRDwoH3+NgTAw+bL/aoOP4DOgH+iwECEt+IlFmkzGHlAYKAWF9R8zUnAAAAAElFTkSuQmCC";
		private const string s_Arrow2 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAC0SURBVDhPjVE5EsIwDMxPKFKYF9CagoJH8xhaMskLmEGsjOSRkBzYmU2s9a58TUQUmCH1BWEHweuKP+D8tphrWcAHuIGrjPnPNY8X2+DzEWE+FzrdrkNyg2YGNNfRGlyOaZDJOxBrDhgOowaYW8UW0Vau5ZkFmXbbDr+CzOHKmLinAXMEePyZ9dZkZR+s5QX2O8DY3zZ/sgYcdDqeEVp8516o0QQV1qeMwg6C91toYoLoo+kNt/tpKQEVvFQAAAAASUVORK5CYII=";
		private const string s_Arrow3 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAAB2SURBVDhPzY1LCoAwEEPnLi48gW5d6p31bH5SMhp0Cq0g+CCLxrzRPqMZ2pRqKG4IqzJc7JepTlbRZXYpWTg4RZE1XAso8VHFKNhQuTjKtZvHUNCEMogO4K3BhvMn9wP4EzoPZ3n0AGTW5fiBVzLAAYTP32C2Ay3agtu9V/9PAAAAAElFTkSuQmCC";
		private const string s_Arrow5 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABqSURBVDhPnY3BCYBADASvFx924NevRdvbyoLBmNuDJQMDGjNxAFhK1DyUQ9fvobCdO+j7+sOKj/uSB+xYHZAxl7IR1wNTXJeVcaAVU+614uWfCT9mVUhknMlxDokd15BYsQrJFHeUQ0+MB5ErsPi/6hO1AAAAAElFTkSuQmCC";
		private const string s_Arrow6 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACaSURBVDhPxZExEkAwEEVzE4UiTqClUDi0w2hlOIEZsV82xCZmQuPPfFn8t1mirLWf7S5flQOXjd64vCuEKWTKVt+6AayH3tIa7yLg6Qh2FcKFB72jBgJeziA1CMHzeaNHjkfwnAK86f3KUafU2ClHIJSzs/8HHLv09M3SaMCxS7ljw/IYJWzQABOQZ66x4h614ahTCL/WT7BSO51b5Z5hSx88AAAAAElFTkSuQmCC";
		private const string s_Arrow7 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAABQSURBVDhPYxh8QNle/T8U/4MKEQdAmsz2eICx6W530gygr2aQBmSMphkZYxqErAEXxusKfAYQ7XyyNMIAsgEkaYQBkAFkaYQBsjXSGDAwAAD193z4luKPrAAAAABJRU5ErkJggg==";
		private const string s_Arrow8 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNWWFMmUAAACYSURBVDhPxZE9DoAwCIW9iUOHegJXHRw8tIdx1egJTMSHAeMPaHSR5KVQ+KCkCRF91mdz4VDEWVzXTBgg5U1N5wahjHzXS3iFFVRxAygNVaZxJ6VHGIl2D6oUXP0ijlJuTp724FnID1Lq7uw2QM5+thoKth0N+GGyA7IA3+yM77Ag1e2zkey5gCdAg/h8csy+/89v7E+YkgUntOWeVt2SfAAAAABJRU5ErkJggg==";
		private const string s_MirrorX = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG1JREFUOE+lj9ENwCAIRB2IFdyRfRiuDSaXAF4MrR9P5eRhHGb2Gxp2oaEjIovTXSrAnPNx6hlgyCZ7o6omOdYOldGIZhAziEmOTSfigLV0RYAB9y9f/7kO8L3WUaQyhCgz0dmCL9CwCw172HgBeyG6oloC8fAAAAAASUVORK5CYII=";
		private const string s_MirrorY = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAG9JREFUOE+djckNACEMAykoLdAjHbPyw1IOJ0L7mAejjFlm9hspyd77Kk+kBAjPOXcakJIh6QaKyOE0EB5dSPJAiUmOiL8PMVGxugsP/0OOib8vsY8yYwy6gRyC8CB5QIWgCMKBLgRSkikEUr5h6wOPWfMoCYILdgAAAABJRU5ErkJggg==";
		private const string s_Rotated = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC41ZYUyZQAAAHdJREFUOE+djssNwCAMQxmIFdgx+2S4Vj4YxWlQgcOT8nuG5u5C732Sd3lfLlmPMR4QhXgrTQaimUlA3EtD+CJlBuQ7aUAUMjEAv9gWCQNEPhHJUkYfZ1kEpcxDzioRzGIlr0Qwi0r+Q5rTgM+AAVcygHgt7+HtBZs/2QVWP8ahAAAAAElFTkSuQmCC";
        
        // Non-standard pluses
        private const string plus1 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAHNJREFUKJFjYKAAMOKSOJ/M8B/GNpyLXR1WwfPJDP8NNBH8C9exG8CC000ySOzr2JUw4dRMBIA7BdmPBppoNj+BOB0GYF5ghGnE0IALPEGEAUXOHmA/o4PzyQz/DVwR/Au7scczRc7GnUieENZMUdqmCAAA4IUj71eaCgsAAAAASUVORK5CYII=";
        private const string plus2 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAGVJREFUKJFjYKAAMOKSODON4T+MbZKFWx1Wjf8fIDCyQciABacJwoQtYSLaOVgA3C/ITjP2QrP5LQPD2W0ILkoYwP34mQiMFAYUOXuA/YwOzkxD9SeueKbI2bgTyVvCmqmftokFAImxUv0RlZm/AAAAAElFTkSuQmCC";
        private const string plus3 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAJBJREFUKJFjYKAAMOKSYI7N+Q9j/108Bas6FlwaOYMT4PzvDAz/sRmAVTMDAwODmL4xnP1w7QKsaphwaSYGwG1G9iOyk2H87wwMGGHACNPIGZyA4lRc4NXFswzf1y5g+Lt4CiNFzqZIMzz40f2M7AWYU2EAV7zDDVK6/x+OkQ2mmrNxJpJXF88S1ExR2qYIAAD1Qzv+t+RnagAAAABJRU5ErkJggg==";
        private const string plus4 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAIpJREFUKJFjYKAAMOKSYPbo/w9j/91RiFUdVkFmj/7/Yhq2cP6rG4exGsCCy2Y9NVk4e88N7GqYcGkmBsBtRvYjspNh/FcMmGHACNMopmGL4lRc4NKtx/AwoMjZFGmGBz+6n5G9AHMqDMD8zIIuwMDAAAkctSi4YlzxTJ2oQgeXbj0mqJmitE0RAAA5qDjrpL7D3gAAAABJRU5ErkJggg==";
        private const string plus5 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAIxJREFUKJFjYKAAMOKSiFCa/x/GXnEvEas6FlwaTQytkETm/8dmAFbNDAwMDCpKanD2mfPHsKphwqWZGAC3GdmPqE6G8THDgBGm0cTQCsWpuMCde7cYzpw/xrDiXiIjRc6mSDM8+NH9jOwFmFNhAOZnFnQBCJj/Hz2qsMUzdaIKHdy5d4ugZorSNkUAAGAkNtY7AcLsAAAAAElFTkSuQmCC";
        private const string plus6 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAJ1JREFUKJGtksENwyAMRR9VcuyFDTJBxQAM0EU6VBbJABkgygTZgAVyoJea0BSrldx/ssz/8L8xGOC0gzuPLPXE2OR1mjD4eDQSuXVBUwww7LdSL8xNzkUT/4Lycp0x+Aj7QQo+QuJjBk6Ewcc3qxq2fmVJMxOjM9k2icv4z5nrCGJVIJm7cwOARB6u1Ve9Mv7VtrokW79+FZt224QnLdcwdhFncngAAAAASUVORK5CYII=";
        private const string plus7 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAIRJREFUKJFjYKAAMOKSWGbi+x/GjjqzGas6rILLTHz/a7LwwPnX/3zBagALLpsVQr0QmpevwqqGCZdmYgDcZmQ/IjsZxscWBowwjZosPChOxQUerN4GDwOKnE2RZnjwo/sZ2Qswp8IAzM8s6AIwgxSQbMAVz9SJKnTwYPU2gpopStsUAQCbkjefDCN5RAAAAABJRU5ErkJggg==";
        // Non-standard minuses
        private const string minus1 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAENJREFUKJFjYBgFJAFGGONmHPd/YjWpL/rKCNd8M477v5rQVwYGfSJ0XmRguPWOm0F90VdGJpLdigQo0kyRn0cBiQAAflQQCLuxhWoAAAAASUVORK5CYII=";
        private const string minus2 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAADpJREFUKJFjYBgFJAFGGOPMNIb/xGoyyULoYzgzjeH//wcM//9/JgI/YPgPs4iJEmdTpJlyP48C4gEA8kQq/rBVTfkAAAAASUVORK5CYII=";
        private const string minus3 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAEZJREFUKJFjYBgFJAFGGIM5Nuc/sZr+Lp7CyMDAwMAC08gZnMAgpm9MUOOri2cZvjMw/P+7eAojExmuhQOKNFPk51FAIgAAiRARvS7kcBwAAAAASUVORK5CYII=";
        private const string minus4 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAEVJREFUKJFjYBgFJAFGGIPZo/8/sZr+7ihkhGtm9uj/L6Zhy6CnJktQ46Vbjxle3TjM8HdHISMTGa6FA4o0U+TnUUAiAAAWYBIIyebnDgAAAABJRU5ErkJggg==";
        private const string minus5 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAEZJREFUKJFjYBgFJAFGGCNCaf5/YjWtuJfIyMDAwMAC02hiaMWgoqRGUOOde7cYGBjm/19xL5GRiXTHIgBFminy8yggEQAAyPERSLurggQAAAAASUVORK5CYII=";
        private const string minus6 = "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAEVJREFUKJFjYBgFJAFGGMOTIe0/sZq2M8xiZGBgYGCBaTQUsmVQ/K1PUON91osMDO8Y/m9nmMXIRIZr4YAizRT5eRSQCAC3Hw54A6LqowAAAABJRU5ErkJggg==";

        private string[] pluses = new string[]
        {
            "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAHNJREFUKJFjYKAAMOKSOJ/M8B/GNpyLXR1WwfPJDP8NNBH8C9exG8CC000ySOzr2JUw4dRMBIA7BdmPBppoNj+BOB0GYF5ghGnE0IALPEGEAUXOHmA/o4PzyQz/DVwR/Au7scczRc7GnUieENZMUdqmCAAA4IUj71eaCgsAAAAASUVORK5CYII=",
            "iVBORw0KGgoAAAANSUhEUgAAAA8AAAAPCAYAAAA71pVKAAAAAXNSR0IArs4c6QAAAGVJREFUKJFjYKAAMOKSODON4T+MbZKFWx1Wjf8fIDCyQciABacJwoQtYSLaOVgA3C/ITjP2QrP5LQPD2W0ILkoYwP34mQiMFAYUOXuA/YwOzkxD9SeueKbI2bgTyVvCmqmftokFAImxUv0RlZm/AAAAAElFTkSuQmCC",
            "",
            "",
            "",
            "",
            "",
        };

        private string[] minuses = new string[]
        {
            "",
        };
        
        private static Texture2D[] s_Arrows;
		public static Texture2D[] arrows
		{
			get
			{
				if (s_Arrows == null)
				{
					s_Arrows = new Texture2D[10];
					s_Arrows[0] = Base64ToTexture(s_Arrow0);
					s_Arrows[1] = Base64ToTexture(s_Arrow1);
					s_Arrows[2] = Base64ToTexture(s_Arrow2);
					s_Arrows[3] = Base64ToTexture(s_Arrow3);
					s_Arrows[5] = Base64ToTexture(s_Arrow5);
					s_Arrows[6] = Base64ToTexture(s_Arrow6);
					s_Arrows[7] = Base64ToTexture(s_Arrow7);
					s_Arrows[8] = Base64ToTexture(s_Arrow8);
					s_Arrows[9] = Base64ToTexture(s_XIconString);
				}
				return s_Arrows;
			}
		}
        private static Texture2D[] t_icons;
        public static Texture2D[] icons
        {
            get
            {
                if (t_icons == null)
                {
                    t_icons = new Texture2D[13];
                    t_icons[0] = Base64ToTexture(plus1);
                    t_icons[1] = Base64ToTexture(plus2);
                    t_icons[2] = Base64ToTexture(plus3);
                    t_icons[3] = Base64ToTexture(plus4);
                    t_icons[4] = Base64ToTexture(plus5);
                    t_icons[5] = Base64ToTexture(plus6);
                    t_icons[6] = Base64ToTexture(plus7);
                    t_icons[7] = Base64ToTexture(minus1);
                    t_icons[8] = Base64ToTexture(minus2);
                    t_icons[9] = Base64ToTexture(minus3);
                    t_icons[10] = Base64ToTexture(minus4);
                    t_icons[11] = Base64ToTexture(minus5);
                    t_icons[12] = Base64ToTexture(minus6);
                }
                return t_icons;
            }
        }


		private static Texture2D[] s_AutoTransforms;
		public static Texture2D[] autoTransforms
		{
			get
			{
				if (s_AutoTransforms == null)
				{
					s_AutoTransforms = new Texture2D[3];
					s_AutoTransforms[0] = Base64ToTexture(s_Rotated);
					s_AutoTransforms[1] = Base64ToTexture(s_MirrorX);
					s_AutoTransforms[2] = Base64ToTexture(s_MirrorY);
				}
				return s_AutoTransforms;
			}
		}
		
		private ReorderableList m_ReorderableList;
		public CustomRuleTile tile { get { return (target as CustomRuleTile); } }
		private Rect m_ListRect;

		public const float k_DefaultElementHeight = 48f;
		public const float k_PaddingBetweenRules = 30f; // was 13
		public const float k_SingleLineHeight = 16f;
		public const float k_LabelWidth = 53f;
        

        public void OnEnable()
		{
            grey.normal.textColor = Color.grey;
            if (!tile.createdOnce) {
                NewRuleTile();
            }
            
            if (tile.m_TilingRules == null)
				tile.m_TilingRules = new List<CustomRuleTile.TilingRule>();

			m_ReorderableList = new ReorderableList(tile.m_TilingRules, typeof(CustomRuleTile.TilingRule), true, true, true, true);
			m_ReorderableList.drawHeaderCallback = OnDrawHeader;
			m_ReorderableList.drawElementCallback = OnDrawElement;
			m_ReorderableList.elementHeightCallback = GetElementHeight;
			m_ReorderableList.onReorderCallback = ListUpdated;
		}

        private void NewRuleTile() {
            tile.createdOnce = true;
            tile.numberOfAccepted = 0;
            tile.acceptedSetSize = new byte[7];
            tile.numberOfDeclined = 0;
            tile.declinedSetSize = new byte[6];
        }

        private void ListUpdated(ReorderableList list)
		{
			SaveTile();
		}

		private float GetElementHeight(int index)
		{
			if (tile.m_TilingRules != null && tile.m_TilingRules.Count > 0)
			{
				switch (tile.m_TilingRules[index].m_Output)
				{
					case CustomRuleTile.TilingRule.OutputSprite.Random:
						return k_DefaultElementHeight + k_SingleLineHeight*(tile.m_TilingRules[index].m_Sprites.Length + 3) + k_PaddingBetweenRules;
					case CustomRuleTile.TilingRule.OutputSprite.Animation:
						return k_DefaultElementHeight + k_SingleLineHeight*(tile.m_TilingRules[index].m_Sprites.Length + 2) + k_PaddingBetweenRules;
				}
			}
			return k_DefaultElementHeight + k_PaddingBetweenRules;
		}

		private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
		{
            CustomRuleTile.TilingRule rule = tile.m_TilingRules[index];

			float yPos = rect.yMin + 2f;
			float height = rect.height - k_PaddingBetweenRules;
			float matrixWidth = k_DefaultElementHeight;
			
			Rect inspectorRect = new Rect(rect.xMin, yPos, rect.width - matrixWidth * 2f - 20f, height);
			Rect matrixRect = new Rect(rect.xMax - matrixWidth * 2f - 10f, yPos, matrixWidth, k_DefaultElementHeight);
			Rect spriteRect = new Rect(rect.xMax - matrixWidth - 5f, yPos, matrixWidth, k_DefaultElementHeight);

			EditorGUI.BeginChangeCheck();
			RuleInspectorOnGUI(inspectorRect, rule);
            RuleMatrixOnGUI(matrixRect, rule, tile);
			SpriteOnGUI(spriteRect, rule);
			if (EditorGUI.EndChangeCheck())
				SaveTile();
		}

		private void SaveTile()
		{
			EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(tile);
			SceneView.RepaintAll();
        }

		private void OnDrawHeader(Rect rect)
		{
			GUI.Label(rect, "Tiling Rules");
		}

		public override void OnInspectorGUI()
		{
            tile.m_DefaultSprite = EditorGUILayout.ObjectField("Default Sprite", tile.m_DefaultSprite, typeof(Sprite), false) as Sprite;
            tile.m_DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", tile.m_DefaultColliderType);

            EditorGUILayout.Space();EditorGUILayout.Space();EditorGUILayout.Space();
            CustomRuleEditor();
            EditorGUILayout.Space();EditorGUILayout.Space();EditorGUILayout.Space();

            if (m_ReorderableList != null && tile.m_TilingRules != null)
				m_ReorderableList.DoLayoutList();
		}



        private void CustomRuleEditor()
        {
            if (tile.acceptedSetSize.Length == 0) tile.acceptedSetSize = new byte[7];
            tile.gTiles = new TileBase[][] { tile.t1, tile.t2, tile.t3, tile.t4, tile.t5, tile.t6, tile.t7 };

            EditorGUILayout.LabelField("Accept rules count");
            tile.numberOfAccepted = (byte)EditorGUILayout.IntSlider(tile.numberOfAccepted, 0, 7);
            for (byte i = 0; i < tile.numberOfAccepted; i++) {
                Rect guiPOS = EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(new Rect(0, guiPOS.y, 15, guiPOS.height), icons[i]);
                EditorGUILayout.LabelField("Tiles");
                EditorGUI.BeginChangeCheck();
                tile.acceptedSetSize[i] = (byte)EditorGUI.DelayedIntField(new Rect(60, guiPOS.y, 30, guiPOS.height), tile.acceptedSetSize[i]);
                if (EditorGUI.EndChangeCheck() || tile.firstFlag) {
                    ResizeTileGroupArray(i, true);
                    tile.firstFlag = false;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                for (byte q = 0; q < tile.acceptedSetSize[i]; q++) {
                    tile.gTiles[i][q] = EditorGUILayout.ObjectField(tile.gTiles[i][q], typeof(TileBase), false) as TileBase;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    SaveTile();
                }
            }

            if (tile.declinedSetSize.Length == 0) tile.declinedSetSize = new byte[6];
            tile.bTiles = new TileBase[][] { tile.n1, tile.n2, tile.n3, tile.n4, tile.n5, tile.n6 };

            EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.LabelField("Decline rules count");
            tile.numberOfDeclined = (byte)EditorGUILayout.IntSlider(tile.numberOfDeclined, 0, 6);
            for (byte i = 0; i < tile.numberOfDeclined; i++) {
                Rect guiPOS = EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(new Rect(0, guiPOS.y, 15, guiPOS.height), icons[i + 7]);
                EditorGUILayout.LabelField("Tiles");
                EditorGUI.BeginChangeCheck();
                tile.declinedSetSize[i] = (byte)EditorGUI.DelayedIntField(new Rect(60, guiPOS.y, 30, guiPOS.height), tile.declinedSetSize[i]);
                if (EditorGUI.EndChangeCheck() || tile.firstFlag2) {
                    ResizeTileGroupArray(i, false);
                    tile.firstFlag2 = false;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                for (byte q = 0; q < tile.declinedSetSize[i]; q++) {
                    tile.bTiles[i][q] = EditorGUILayout.ObjectField(tile.bTiles[i][q], typeof(TileBase), false) as TileBase;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    SaveTile();
                }
            }
        }

        private void ResizeTileGroupArray(byte i, bool good)
        {
            if (good)
            {
                switch (i)
                {
                    case 0:
                        Array.Resize(ref tile.t1, tile.acceptedSetSize[i]);
                        break;
                    case 1:
                        Array.Resize(ref tile.t2, tile.acceptedSetSize[i]);
                        break;
                    case 2:
                        Array.Resize(ref tile.t3, tile.acceptedSetSize[i]);
                        break;
                    case 3:
                        Array.Resize(ref tile.t4, tile.acceptedSetSize[i]);
                        break;
                    case 4:
                        Array.Resize(ref tile.t5, tile.acceptedSetSize[i]);
                        break;
                    case 5:
                        Array.Resize(ref tile.t6, tile.acceptedSetSize[i]);
                        break;
                    case 6:
                        Array.Resize(ref tile.t7, tile.acceptedSetSize[i]);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        Array.Resize(ref tile.n1, tile.declinedSetSize[i]);
                        break;
                    case 1:
                        Array.Resize(ref tile.n2, tile.declinedSetSize[i]);
                        break;
                    case 2:
                        Array.Resize(ref tile.n3, tile.declinedSetSize[i]);
                        break;
                    case 3:
                        Array.Resize(ref tile.n4, tile.declinedSetSize[i]);
                        break;
                    case 4:
                        Array.Resize(ref tile.n5, tile.declinedSetSize[i]);
                        break;
                    case 5:
                        Array.Resize(ref tile.n6, tile.declinedSetSize[i]);
                        break;
                    default:
                        break;
                }
            }
        }


        internal static void RuleMatrixOnGUI(Rect rect, CustomRuleTile.TilingRule tilingRule, CustomRuleTile tile)
		{
			Handles.color = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.2f) : new Color(0f, 0f, 0f, 0.2f);
			int index = 0;
			float w = rect.width / 3f;
			float h = rect.height / 3f;

			for (int y = 0; y <= 3; y++)
			{
				float top = rect.yMin + y * h;
				Handles.DrawLine(new Vector3(rect.xMin, top), new Vector3(rect.xMax, top));
			}
			for (int x = 0; x <= 3; x++)
			{
				float left = rect.xMin + x * w;
				Handles.DrawLine(new Vector3(left, rect.yMin), new Vector3(left, rect.yMax));
			}
			Handles.color = Color.white;

			for (int y = 0; y <= 2; y++)
			{
				for (int x = 0; x <= 2; x++)
				{
					Rect r = new Rect(rect.xMin + x * w, rect.yMin + y * h, w - 1, h - 1);
					if (x != 1 || y != 1)
					{
                        try
                        {
                            switch (tilingRule.m_Neighbors[index])
						    {
							    case CustomRuleTile.TilingRule.Neighbor.This:
								    GUI.DrawTexture(r, arrows[y*3 + x]);
								    break;
							    case CustomRuleTile.TilingRule.Neighbor.NotThis:
								    GUI.DrawTexture(r, arrows[9]);
								    break;
                                case CustomRuleTile.TilingRule.Neighbor.DontCare:
                                    break;
                                default:
                                    GUI.DrawTexture(r, icons[(int)tilingRule.m_Neighbors[index] - 3]);
                                    break;
                            }
                        } catch (System.IndexOutOfRangeException)
                        { }
						if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
						{
                            /////int change = 1;
                            if (Event.current.button == 0)
                            {
                                /////change = -1;
                                tilingRule.m_Neighbors[index] = (CustomRuleTile.TilingRule.Neighbor) 
                                    GetNextIcon((int)tilingRule.m_Neighbors[index], tile.numberOfAccepted, tile.numberOfDeclined);
                                GUI.changed = true;
                                Event.current.Use();
                            }
						}

						index++;
					}
					else
					{
						switch (tilingRule.m_RuleTransform)
						{
							case CustomRuleTile.TilingRule.Transform.Rotated:
								GUI.DrawTexture(r, autoTransforms[0]);
								break;
							case CustomRuleTile.TilingRule.Transform.MirrorX:
								GUI.DrawTexture(r, autoTransforms[1]);
								break;
							case CustomRuleTile.TilingRule.Transform.MirrorY:
								GUI.DrawTexture(r, autoTransforms[2]);
								break;
						}

						if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
						{
							tilingRule.m_RuleTransform = (CustomRuleTile.TilingRule.Transform)(((int)tilingRule.m_RuleTransform + 1) % 4);
							GUI.changed = true;
							Event.current.Use();
						}
					}
				}
			}
		}

        private static void OnSelect(object userdata)
		{
			MenuItemData data = (MenuItemData) userdata;
			data.m_Rule.m_RuleTransform = data.m_NewValue;
		}

		private class MenuItemData
		{
			public CustomRuleTile.TilingRule m_Rule;
			public CustomRuleTile.TilingRule.Transform m_NewValue;

			public MenuItemData(CustomRuleTile.TilingRule mRule, CustomRuleTile.TilingRule.Transform mNewValue)
			{
				this.m_Rule = mRule;
				this.m_NewValue = mNewValue;
			}
		}

		///*
        internal static void SpriteOnGUI(Rect rect, CustomRuleTile.TilingRule tilingRule)
		{
			tilingRule.m_Sprites[0] = EditorGUI.ObjectField(new Rect(rect.xMax - rect.height, rect.yMin, rect.height, rect.height), tilingRule.m_Sprites[0], typeof (Sprite), false) as Sprite;
		}

        internal static void RuleInspectorOnGUI(Rect rect, CustomRuleTile.TilingRule tilingRule)
		{
			float y = rect.yMin;
			EditorGUI.BeginChangeCheck();
			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Rule");
			tilingRule.m_RuleTransform = (CustomRuleTile.TilingRule.Transform)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_RuleTransform);
			y += k_SingleLineHeight;
			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Collider");
			tilingRule.m_ColliderType = (Tile.ColliderType)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_ColliderType);
			y += k_SingleLineHeight;
			GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Output");
			tilingRule.m_Output = (CustomRuleTile.TilingRule.OutputSprite)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Output);
			y += k_SingleLineHeight;

			if (tilingRule.m_Output == CustomRuleTile.TilingRule.OutputSprite.Animation)
			{
				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Speed");
				tilingRule.m_AnimationSpeed = EditorGUI.FloatField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_AnimationSpeed);
				y += k_SingleLineHeight;
			}
			if (tilingRule.m_Output == CustomRuleTile.TilingRule.OutputSprite.Random)
			{
				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Noise");
				tilingRule.m_PerlinScale = EditorGUI.Slider(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_PerlinScale, 0.001f, 0.999f);
				y += k_SingleLineHeight;

				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Shuffle");
				tilingRule.m_RandomTransform = (CustomRuleTile.TilingRule.Transform)EditorGUI.EnumPopup(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_RandomTransform);
				y += k_SingleLineHeight;
			}

			if (tilingRule.m_Output != CustomRuleTile.TilingRule.OutputSprite.Single)
			{
				GUI.Label(new Rect(rect.xMin, y, k_LabelWidth, k_SingleLineHeight), "Size");
				EditorGUI.BeginChangeCheck();
				int newLength = EditorGUI.DelayedIntField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites.Length);
				if (EditorGUI.EndChangeCheck())
					Array.Resize(ref tilingRule.m_Sprites, Math.Max(newLength, 1));
				y += k_SingleLineHeight;

				for (int i = 0; i < tilingRule.m_Sprites.Length; i++)
				{
					tilingRule.m_Sprites[i] = EditorGUI.ObjectField(new Rect(rect.xMin + k_LabelWidth, y, rect.width - k_LabelWidth, k_SingleLineHeight), tilingRule.m_Sprites[i], typeof(Sprite), false) as Sprite;
					y += k_SingleLineHeight;
				}
			}
		}
        //*/
		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			if (tile.m_DefaultSprite != null)
			{
				Type t = GetType("UnityEditor.SpriteUtility");
				if (t != null)
				{
					MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] {typeof (Sprite), typeof (Color), typeof (int), typeof (int)});
					if (method != null)
					{
						object ret = method.Invoke("RenderStaticPreview", new object[] {tile.m_DefaultSprite, Color.white, width, height});
						if (ret is Texture2D)
							return ret as Texture2D;
					}
				}
			}
			return base.RenderStaticPreview(assetPath, subAssets, width, height);
		}

		private static Type GetType(string TypeName)
		{
			var type = Type.GetType(TypeName);
			if (type != null)
				return type;

			if (TypeName.Contains("."))
			{
				var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
				var assembly = Assembly.Load(assemblyName);
				if (assembly == null)
					return null;
				type = assembly.GetType(TypeName);
				if (type != null)
					return type;
			}

			var currentAssembly = Assembly.GetExecutingAssembly();
			var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
			foreach (var assemblyName in referencedAssemblies)
			{
				var assembly = Assembly.Load(assemblyName);
				if (assembly != null)
				{
					type = assembly.GetType(TypeName);
					if (type != null)
						return type;
				}
			}
			return null;
		}

		private static Texture2D Base64ToTexture(string base64)
		{
			Texture2D t = new Texture2D(1, 1);
			t.hideFlags = HideFlags.HideAndDontSave;
			t.LoadImage(System.Convert.FromBase64String(base64));
			return t;
		}
        
        private static int GetNextIcon(int current, byte acceptedCount, byte declinedCount) { 
            current++;
            if (current > 15)
                current = 0;
            if (current < 3 + acceptedCount)
                return current;
            if (current > 9 && current < 10 + declinedCount)
                return current;
            return GetNextIcon(current, acceptedCount, declinedCount);
        }

    }
}
