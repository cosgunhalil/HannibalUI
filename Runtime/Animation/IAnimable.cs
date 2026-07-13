namespace HannibalUI.Runtime.Animation
{
    public interface IAnimable
    {
        void PlayForward(float activationTime);
        void PlayRewind(float deactivationTime);
    }
}
