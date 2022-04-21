using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimCntrler : MonoBehaviour
{
    public SkeletonAnimation sAnimation;

    /*Fire animation as necessary
     * 
     * The index of each animation should be as follows:
     * 0: Attack
     * 1: Dead
     * 2: Idle //Placeholder until we can determine a solution for being hit
     * 3: Walk
     * This should be the same for each pre-built enemy using the spine animation toolkit, for those instances where it is not adjustments may be needed
     * 
     */

    public void DoSpineAnim(int indx, string name, bool loop = false)
    {
        if(indx != 3)
        {
            if (sAnimation.AnimationState.GetCurrent(indx) == null)
            {
                StopAllSpineAnim();   
                sAnimation.AnimationState.SetAnimation(indx, name, loop);
            }
        }
        else
        {
            QueryCompleteAnim(0);
            QueryCompleteAnim(2);

            if (sAnimation.AnimationState.GetCurrent(0) == null && sAnimation.AnimationState.GetCurrent(2) == null)
            {
                if (sAnimation.AnimationState.GetCurrent(indx) == null)
                {
                    sAnimation.AnimationState.SetAnimation(indx, name, loop);
                }
                else if (sAnimation.AnimationState.GetCurrent(indx).IsComplete)
                {
                    sAnimation.AnimationState.SetAnimation(indx, name, loop);
                }
            }
        }
    }

    public void StopSpineAnim(int indx)
    {
        sAnimation.AnimationState.ClearTrack(indx);
    }

    public void StopAllSpineAnim()
    {
        sAnimation.AnimationState.ClearTrack(3);
        sAnimation.AnimationState.ClearTrack(2);
        sAnimation.AnimationState.ClearTrack(1);
        sAnimation.AnimationState.ClearTrack(0);
    }

    void QueryCompleteAnim(int trackindx)
    {
        if (sAnimation.AnimationState.GetCurrent(trackindx) != null && sAnimation.AnimationState.GetCurrent(trackindx).IsComplete)
        {
            sAnimation.AnimationState.ClearTrack(trackindx);
        }
    }
}
