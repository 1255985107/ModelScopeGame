# 视频文件夹

将你的介绍视频文件放在这个文件夹中。

## 支持的视频格式
- MP4 (推荐)
- MOV  
- AVI
- WEBM

## 推荐设置
- 分辨率: 1920x1080 或更低
- 编码: H.264
- 音频: AAC
- 帧率: 30fps
- 配置文件：Baseline 或 Main
- 比特率: 5-10 Mbps
- 像素格式：yuv420p

## 使用FFmpeg重新编码视频为Unity兼容格式

```bash
ffmpeg -i input_video.mp4 -c:v libx264 -profile:v baseline -level 3.0 -pix_fmt yuv420p -c:a aac -b:a 128k output_video.mp4
```

## 现有视频链接

Intro.mp4: https://drive.google.com/file/d/1Q_OBv4VIhouiqx3gmqHje11H3MNa3wKH/view?usp=sharing
Chapter0.mp4: https://drive.google.com/file/d/1O_Veg285QLRXzOMgJ4GIJGb2rIr0N4ea/view?usp=sharing
Chapter1.mp4: https://drive.google.com/file/d/14-FbH_D92ujqsxx5rckwveeh3PcZ2yuk/view?usp=sharing
Chapter2.mp4: https://drive.google.com/file/d/1nC9-o_1fe-hUs3IEZ2ktn4-eQ_Bxh74O/view?usp=sharing