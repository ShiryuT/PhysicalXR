import processing.serial.*;
import cc.arduino.*;
import org.firmata.*;

import oscP5.*;
import netP5.*;

Arduino arduino;
int sensorPin = 0;
int slopePin = 1;

int flag=0;
float avatarz = 0;

boolean f = true;
boolean f2 = true;

OscP5 oscP5;
NetAddress myRemoteLocation;

void setup(){
  arduino = new Arduino(this,Arduino.list()[5],57600);
  arduino.pinMode(sensorPin,Arduino.INPUT);
  arduino.pinMode(slopePin,Arduino.INPUT);
  size(800,200);
  frameRate(10);
  background(0);
  
  oscP5 = new OscP5(this,5555);
  myRemoteLocation = new NetAddress("127.0.0.1",6666);
  
  oscP5.plug(this,"GetFlag","/flag");
  oscP5.plug(this,"GetAvatar","/avatar");
  
}

void draw(){
  background(0);
  reset();
  float val = arduino.analogRead(sensorPin);
  float val2 = arduino.analogRead(slopePin);
  
  //Unityに圧力センサ、加速度センサの値を送信
  OscMessage myMessage1 = new OscMessage("/touch");
  myMessage1.add(int(val));
  oscP5.send(myMessage1,myRemoteLocation);
  OscMessage myMessage2 = new OscMessage("/slope");
  myMessage2.add(int(val2));
  oscP5.send(myMessage2,myRemoteLocation);
  
  //flagの状態に応じて描画変更
  if (flag==1)
  {
    fill(255,0,0);
    triangle(165,150,120,135,120,165);
    fill(252);
    ellipse(135, 110, 10, 10);
    
  }
  if (flag==2)
  {
    if(f==true)
    {
      delay(3000);
      f=false;
    }
    fill(255,0,0);
    triangle(165+avatarz*2.65, 150, 120+avatarz*2.65, 135, 120+avatarz*2.65, 165);
    fill(252);
    ellipse(400, 50, 10, 10);
    f2=true;
  
  }
  if (flag==3)
  {
    fill(255,0,0);
    triangle(635, 150, 680, 135, 680, 165);
    fill(252);
    ellipse(665, 110, 10, 10);
    
  }
  if (flag==4)
  {
    fill(255,0,0);
    triangle(105+avatarz*2.65, 150, 150+avatarz*2.65, 135, 150+avatarz*2.65, 165);
    fill(252);
    ellipse(400, 50, 10, 10);
  }
  if (flag==5)
  {
    fill(255,0,0);
    triangle(165,150,120,135,120,165);
    fill(252);
    ellipse(135, 110, 10, 10);
  }
  if (flag==6)
  {
    fill(255,0,0);
    triangle(165,150,120,135,120,165);
    fill(252);
    ellipse(135, 110, 10, 10);
    f=true;
  }
  
  
}

//Unityからflagの状態を受信
void GetFlag(int nflag){
  flag=nflag;
  
}
//Unityからアバターのz座標を受信
void GetAvatar(float x, float y, float z){

  avatarz=z; 
}

//基礎の描画
void reset()
{
  background(0);
  stroke(255);
  strokeWeight(3);
  line(150, 150, 650,150);
  noFill();
  ellipse(135,150,30,30);
  ellipse(665,150,30,30);
  arc(400, 110, 530, 120, PI, TWO_PI);
  
  //最初だけアバター位置となる三角も描画
  if (flag==0)
  {
    fill(255,0,0);
    triangle(165,150,120,135,120,165);
    fill(252);
    ellipse(135, 110, 10, 10);
  }
}
