using UnityEngine;

namespace QuickEye.NestedAssetList.Demo
{
	public class Spell : ScriptableObject
	{
		public NestedAssetList<SpellEffect> effects;
	}
}