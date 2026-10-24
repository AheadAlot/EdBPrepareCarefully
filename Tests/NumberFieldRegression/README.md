# 数字输入控件回归测试

```sh
dotnet run --project Tests/NumberFieldRegression -c Release
```

测试直接编译生产代码 `DragSlider.cs` 和 `WidgetNumberField.cs`，以最小 GUI / 游戏替身模拟鼠标事件和按钮状态，无需游戏安装。

覆盖年龄和装备数量的快速点击、输入与回车提交、松键后移动、正常按住拖动、已消费或丢失的松键事件、右键释放、页面清理及非本 Mod 拖动不受干扰。页面切换 / 关闭调用点及真实 1.7 窗口事件分发仍需游戏内验证。
