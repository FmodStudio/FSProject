
namespace ThGold.Feel
{
    public class FeedBackCustomName
    {

        /// <summary>
        /// lets you load a destination scene using various methods, either native or using MMTools�� loading screens
        /// ������ʹ�ø��ַ�������Ŀ�곡����������ԭ����������ʹ�� MMTools �ļ�����Ļ
        /// </summary>
        public const string FeedBack_LoadScene = "FeedBack_LoadScene";
        /// <summary>
        /// ж�س���
        /// </summary>
        public const string FeedBack_UnLoadScene = "FeedBack_UnLoadScene";
        /// <summary>
        /// �����ڲ��ŷ���ʱ�Ŵ����С����������ָ���ĳ���ʱ���ڣ�����ֱ������֪ͨ��
        /// �����ʹ�õ�����ͨ���������Ҫһ�� MMCameraZoom ����������ʹ�õ��� Cinemachine �����������
        /// ����Ҫһ�� MMCinemachineZoom ������˷�����������ʹ�����ֵ����ӵ���������ʱ�ĵ�ǰ���š�
        ///  lets you zoom in or out when the feedback plays, either for a specified duration, or until further notice. 
        ///  If you��re using a regular camera, you will need a MMCameraZoom component on it.
        ///  If you��re using a Cinemachine virtual camera, you��ll need a MMCinemachineZoom component on it.
        /// </summary>
        public const string FeedBack_ChineZoom = "FeedBack_ChineZoom";
        /// <summary>
        /// flash an image on screen, or simply a color for a short duration. 
        /// You��ll need an element (or more) with a MMFlash component on it in your scene for this feedback to interact with. 
        /// </summary>
        public const string FeedBack_Bloom = "FeedBack_Bloom";
        /// <summary>
        /// ades an image in or out, useful for transitions. 
        /// This feedback requires an object with an MMFader component on it in your scene. 
        /// </summary>
        public const string FeedBack_Fade = "FeedBack_Fade";
        /// <summary>
        /// �ı侰��
        /// control a camera��s field of view over time. Will require a MMCameraFieldOfViewShaker on your camera, 
        /// or a MMCinemachineFieldOfViewShaker on your virtual camera.
        /// </summary>
        public const string FeedBack_FieldOfView = "FeedBack_FieldOfView";
        /// <summary>
        /// lets you transition to another virtual camera, using the blend of your choice, and auto managing other camera��s priorities.
        /// You��ll need a MMCinemachinePriorityListener on each of your virtual cameras for this to work. 
        /// If you want more control, you can also add a MMCinemachinePriorityBrainListener on your Cinemachine brain. 
        /// This will let you specify the transition to use to override the default one straight from the feedback.
        /// </summary>
        public const string FeedBack_CineTransition = "FeedBack_CineTransition";
        /// <summary>
        /// ɫ��
        /// ����ʱ������ƿ���ɫ������������ĺ��ڴ��������Ҫһ�� MMChromaticAberrationShaker
        /// You��ll need a MMChromaticAberrationShaker on your post processing volume
        /// </summary>
        public const string FeedBack_ChromaticAberration = "FeedBack_ChromaticAberration";
        /// <summary>
        /// ����
        /// lets you control depth of field focus distance, aperture and focal length over time. You��ll need a MMDepthOfFieldShaker on your post processing volume
        /// </summary>
        public const string FeedBack_DepthOfField = "FeedBack_DepthOfField";
        /// <summary>
        /// ��ͷ����
        /// lens distortion on demand. You��ll need a MMLensDistortionShaker on your post processing volume.
        /// </summary>
        public const string FeedBack_LensDistortion = "FeedBack_LensDistortion";
        /// <summary>
        /// ����ʱ������ƿ�����Ӱ���������ĺ��ڴ��������Ҫһ�� MMVignetteShaker
        /// </summary>
        public const string FeedBack_Vignette = "FeedBack_Vignette";
        /// <summary>
        /// �������Ӷ���
        /// </summary>
        public const string FeedBack_PlayParticles = "FeedBack_PlayParticles";
    }
}
