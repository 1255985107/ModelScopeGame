## ModelScope 课程综合实践

资源结构：
- Resources 下存放图片、预制体等资源
- Scenes 下存放场景
- Scripts 下存放 C# 脚本
- Sound 下存放音频
- Texture 下存放贴图纹理
- 第三方插件（如有）文件夹直接放在 Assets 下

注意事项：
- 两个人不要同时更改同一个场景，否则面临 scene.unity 文件冲突
	- 补救方法：先将两方添加的内容都接受，再进入 unity editor 中去掉重复或多余的