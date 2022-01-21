using System;
using UnityEngine;
using System.Collections.Generic;

/*
 * http://forum.unity3d.com/threads/29871-Number-of-bind-poses-doesn-t-match-number-of-bones-in
 * 
 */

static class SMRendererToGameObjectCopier
{
    public static GameObject [] CopySMRendererToGameObject(GameObject target, GameObject gameObjectToCombine)
    {
        // Retrieve all skinned mesh renderers from this game object and its children
        Component[] comps = gameObjectToCombine.GetComponentsInChildren(typeof(SkinnedMeshRenderer), true);

        List<GameObject> instances = new List<GameObject>();

        foreach (Component comp in comps)
        {
            // Copy this skinned mesh renderer to the target game object
            GameObject inst = CopySMRendererEx((SkinnedMeshRenderer)comp, target);
            instances.Add(inst);
        }

        return instances.ToArray();
    }

    /// <summary>
    /// Takes a target GameObject and an array of GameObjects. All skinned mesh renderers attached to any GameObject in the array are copied to the target GameObject.
    /// </summary>

    public static void CopySMRenderersToGameObject(GameObject target, GameObject[] gameObjectsToCombine)
    {
        // Record start time for logging time spent
        float startTime = Time.realtimeSinceStartup;

        // Step through all game objects in gameObjectsToCombine
        foreach (GameObject go in gameObjectsToCombine)
        {
            // Retrieve all skinned mesh renderers from this game object and its children
            Component[] comps = go.GetComponentsInChildren(typeof(SkinnedMeshRenderer), true);

            foreach (Component comp in comps)
            {
                // Copy this skinned mesh renderer to the target game object
                CopySMRendererEx((SkinnedMeshRenderer)comp, target);
            }
        }

        // Log time spent
        Debug.Log("Combining game objects took " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
    }

    static void CopySMRenderer(SkinnedMeshRenderer renderer, GameObject target)
    {
        // Create a new game object to avoid overwriting the skinned mesh renderer component as we will add them to the same game object
        GameObject go = new GameObject("Merged Geometry: " + renderer.name);

        // Create a new skinned mesh renderer
        SkinnedMeshRenderer smr = (SkinnedMeshRenderer)go.AddComponent(typeof (SkinnedMeshRenderer));

        // Copy the mesh to the new game object
        smr.sharedMesh = renderer.sharedMesh;

        // Copy the material to the new game object
        smr.sharedMaterials = renderer.sharedMaterials;
		
		smr.useLightProbes = renderer.useLightProbes;
		smr.updateWhenOffscreen = renderer.updateWhenOffscreen;
		
		
        // To remap the skinning to the target skelleton we need to create an array that holds references to the bones in the target skelleton
        // that correspond to the bones the original renderer is using. We find the bones by name.

        // Create an array to store references to the bones in the target skelleton
        Transform[] bones = new Transform[renderer.bones.Length];

        // Step through all bones used by the original renderer
        for (int bone = 0; bone < renderer.bones.Length; bone++)
        {
            Transform t = null;

            // Step through all transforms in the target game object
            foreach (Component c in target.GetComponentsInChildren(typeof(Transform)))
            {
                // Check if we have found the corresponding bone
                if (c.name == renderer.bones[bone].name)
                {
                    t = c.transform;
                    break;
                }
            }

            // Throw an exception when we dont find a bone with the same name
            if (t == null) throw new Exception("Bone not found: " + renderer.bones[bone].name);

            // Add the bone we found to the array
            bones[bone] = t;
        }

        // Set the bones array to make the new skinned mesh renderer use the target skelleton
        smr.bones = bones;

        // Attach the new game object holding the new skinned mesh renderer to the target game object
        go.transform.parent = target.transform;
    }
	
	static GameObject CopySMRendererEx(SkinnedMeshRenderer renderer, GameObject target)
	{
		/*
        // Create a new game object to avoid overwriting the skinned mesh renderer component as we will add them to the same game object
        GameObject go = new GameObject("Merged Geometry: " + renderer.name);
		
        // Create a new skinned mesh renderer
        SkinnedMeshRenderer smr = (SkinnedMeshRenderer)go.AddComponent(typeof (SkinnedMeshRenderer));


        // Copy the mesh to the new game object
        smr.sharedMesh = renderer.sharedMesh;

        // Copy the material to the new game object
        smr.sharedMaterials = renderer.sharedMaterials;
		
		smr.useLightProbes = renderer.useLightProbes;
		smr.updateWhenOffscreen = renderer.updateWhenOffscreen;
		*/

		GameObject go = GameObject.Instantiate(renderer.gameObject) as GameObject;

        
		SkinnedMeshRenderer smr = (SkinnedMeshRenderer)go.GetComponent(typeof (SkinnedMeshRenderer));
        
        //Note. Download Shader Error fixed
        foreach (var m in smr.sharedMaterials)
        {
            if (!m.shader.name.StartsWith("Transparent/"))
            {
                int instid = m.GetInstanceID();
                m.shader = Shader.Find(m.shader.name);
            }
        }
        
        // To remap the skinning to the target skelleton we need to create an array that holds references to the bones in the target skelleton
        // that correspond to the bones the original renderer is using. We find the bones by name.

        // Create an array to store references to the bones in the target skelleton
        Transform[] bones = new Transform[renderer.bones.Length];

        // Step through all bones used by the original renderer
        for (int bone = 0; bone < renderer.bones.Length; bone++)
        {
            Transform t = null;

            // Step through all transforms in the target game object
            foreach (Component c in target.GetComponentsInChildren(typeof(Transform)))
            {
                // Check if we have found the corresponding bone
                if (c.name == renderer.bones[bone].name)
                {
                    t = c.transform;
                    break;
                }
            }

            // Throw an exception when we dont find a bone with the same name
            if (t == null)
            {
                throw new Exception("Bone not found: " + renderer.bones[bone].name);
            }

            // Add the bone we found to the array
            bones[bone] = t;
        }

        // Set the bones array to make the new skinned mesh renderer use the target skelleton
        smr.bones = bones;

        // Attach the new game object holding the new skinned mesh renderer to the target game object
        go.transform.parent = target.transform;

        return go;
	}
}