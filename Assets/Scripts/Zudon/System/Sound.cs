using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {
	// BGM・SEファイル
	public AudioClip GameBGM_Intro;
	public AudioClip GameBGM_Loop;
	
	public AudioClip bombSE;
	public AudioClip hitSE;
	public AudioClip setSE;
	public AudioClip shootSE;
	
	public AudioClip startSE;
	
	// 音量
	public float BGMvolume = 1.0f;
	public float SEvolume = 1.0f;
	public bool mute = false;
	
	
	public int playSEmaxNum = 16;		// SE同時再生可能数
	private AudioSource[] BGMsources;	// 0:IntroもしくはLoopなし, 1:Loop用
	private AudioSource[] SEsources;

	private AudioClip[] BGM;
	private AudioClip[] SE;
	
	private void Awake () {
		BGMsources = new AudioSource[2];
		for(int i = 0; i < 2; i++){
			BGMsources[i] = gameObject.AddComponent<AudioSource>();
		}
		
		SEsources = new AudioSource[playSEmaxNum];
		for(int i = 0; i < playSEmaxNum; i++){
			SEsources[i] = gameObject.AddComponent<AudioSource>();
		}
		
		BGMsources[1].loop = true;
	}
	
	private void Start () {
		SetVolume();
	}
		
	// 音量・ミュート設定
	public void SetVolume () {
		foreach(AudioSource source in BGMsources){
			source.mute = mute;
			source.volume = BGMvolume;
		}
		
		foreach(AudioSource source in SEsources){
			source.mute = mute;
			source.volume = SEvolume;
		}
	}
	

	// BGM再生
	public void PlayBGM (AudioClip BGMclip) {
		SetVolume();
		if(BGMsources[0].clip == BGMclip ||
		   BGMsources[1].clip == BGMclip)
			return;
		
		StopBGM();
		BGMsources[0].clip = BGMclip;
		BGMsources[0].Play();
	}
	
	// ループ付きBGM再生
	public void PlayBGM (AudioClip BGMclip_Intro, AudioClip BGMclip_Loop) {
		SetVolume();
		if(BGMsources[0].clip == BGMclip_Intro ||
		   BGMsources[1].clip == BGMclip_Loop)
		   return;
		
		StopBGM();
		
		BGMsources[0].clip = BGMclip_Intro;
		BGMsources[1].clip = BGMclip_Loop;
		
		BGMsources[0].Play();
		BGMsources[1].PlayDelayed(BGMsources[0].clip.length);
	}
	
	// BGM停止
	public void StopBGM () {
		foreach(AudioSource source in BGMsources){
			source.Stop();
			source.clip = null;
		}
	}
	
	// BGMフェードアウト
	public void FadeOutBGM (int fadeoutBGMTime) {
		foreach(AudioSource source in BGMsources){
			float volume = source.volume - (float)1 / fadeoutBGMTime;
			if (volume < 0) volume = 0;
			source.volume = volume;
		}
	}
	
	// SE再生(再生中でないAudioSouceで鳴らす)
	public void PlaySE (AudioClip SEclip) {
		SetVolume();
		foreach(AudioSource source in SEsources){
			if(!source.isPlaying){
				source.clip = SEclip;
				source.Play();
				return;
			}
		}  
	}
	
	// SE停止
	public void StopSE () {
		foreach(AudioSource source in SEsources){
			source.Stop();
			source.clip = null;
		}  
	}
}
