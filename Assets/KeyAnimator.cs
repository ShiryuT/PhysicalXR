using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using OscJack;

public class KeyAnimator : MonoBehaviour
{
    public GameObject oscobj=null;
    private OSCManager oscmng;
    public int touchl;
    public int slopel;
    public float speed=0;

    //アバターのz座標
    private float zPosition = 0;
    Animator animator;
    private bool turncheck = true;
    private bool fturn = true;

    private int lastFlag;
    [SerializeField] string ipAddress = "127.0.0.1";
    [SerializeField] int port = 5555;
    OscClient client;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("aflag",0);

        oscmng = (OSCManager)oscobj.GetComponent("OSCManager");

        client = new OscClient(ipAddress,port);

    }

    // Update is called once per frame
    void Update()
    {   //圧力センサーの値
        touchl=oscmng.touch;
        //加速度センサーの値
        slopel=oscmng.slope;

        int currentFlag = animator.GetInteger("aflag");
        //Processingへ送信、変数更新(flagが更新されるたび送信)
        if (currentFlag != lastFlag)
        {
            client.Send("/flag", currentFlag);

            lastFlag = currentFlag;
        }

        //アバターの向きの制御
        if(fturn==true){
            turncheck=false;
            fturn=false;
        }
        else{
            turncheck=true;
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("WarmingUp"))
        {
            //圧力センサーを握ったらモーション変更
            if(touchl>300){
                animator.SetInteger("aflag",1);
            }
        }
         if(animator.GetCurrentAnimatorStateInfo(0).IsName("BaseballPitching"))
        {
            //投げるモーションをしてそのままモーション変更
            animator.SetInteger("aflag",2);
            //走っている向きの判定変数
            animator.SetBool("left",true);

        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("StandardRun")){
            bool left = animator.GetBool("left");


            //右向きに走っている時加速度センサの反応に応じて走る速度の変更
            if(left==false){

                if(slopel<320){
                    speed=Mathf.Min((320-slopel)/10,13);

                    zPosition=transform.position.z;

                    //初期位置に戻ってきたらモーション変更
                    if(zPosition<0){
                        speed=0;
                        animator.SetInteger("aflag",5);
                        animator.SetBool("left",true);
                        if(turncheck==true){
                            this.transform.Rotate(0,180,0);
                            turncheck=false;
                        }
                    }
                }
                //走っている間の座標変更
                Vector3 pos0 = this.transform.position;
                Vector3 pos1 = new Vector3(pos0.x,pos0.y,pos0.z-speed*0.01f);
                this.transform.position=pos1;
                //走っているモーションのスピード変更
                animator.speed = 1+speed*0.05f;
                //カメラもアバターに追従
                GameObject camera = GameObject.Find("Main Camera");
                camera.transform.Translate(-speed*0.01f, 0, 0);
            }

            //左向きに走っている時加速度センサの反応に応じて走る速度の変更
            if(left==true){

                if(slopel>340){
                    speed=Mathf.Min((slopel-340)/10,13);

                    zPosition=transform.position.z;

                    //指定の位置に来たらモーション変更
                    if(zPosition>200){
                        speed=0;
                        animator.SetInteger("aflag",3);
                        animator.SetBool("left",false);
                        if(turncheck==true){
                            this.transform.Rotate(0,180,0);
                            turncheck=false;
                        }
                    }

                }
                //走っている間の座標変更
                Vector3 pos0 = this.transform.position;
                Vector3 pos1 = new Vector3(pos0.x,pos0.y,pos0.z+speed*0.01f);
                this.transform.position=pos1;
                //走っているモーションのスピード変更
                animator.speed = 1+speed*0.05f;
                //カメラもアバターに追従
                GameObject camera = GameObject.Find("Main Camera");
                camera.transform.Translate(speed*0.01f, 0, 0);
            }

        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("BaseballStrike")){
            //打つモーションをしてそのままモーション変更
            animator.SetInteger("aflag",4);
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("NReciverCatch")){
            //圧力センサーを握ったらモーション変更
            if(touchl>300){
                animator.SetInteger("aflag",6);
            }

        }

    }

    public void SetSpeed(float val){
        this.speed = val;
    }
    void OnDisable()
    {
        client.Dispose();
    }

}
