# EdB Prepare Carefully 简繁中文汉化独立版

`cn-standalone` 分支提供自带简繁中文翻译的完整 Mod，不需要原版 Prepare Carefully。
同时支持 **RimWorld 1.6 和 1.7**，分别使用独立程序集。需要启用支持对应游戏版本的 **Harmony**，并将其排在本 Mod 之前。

## 构建

需要 .NET 10 SDK 和首次还原 NuGet 包时的网络连接。项目同时编译 .NET Framework 4.7.2（1.6）与 .NET 10（1.7），输出目录相互隔离。

1.6 默认使用固定版本的 `Krafs.Rimworld.Ref` 1.6.4871 参考程序集；也可设置 `RimWorld16ManagedDir`，改用本机 1.6 的 `RimWorldWin64_Data/Managed`。1.7 已迁移到 .NET 10，游戏程序集位于游戏安装目录。

```bat
dotnet build -c Release -p:RimWorldDir="E:\Steam\steamapps\common\RimWorld" -p:HarmonyPath="E:\Steam\steamapps\workshop\content\294100\2009463077\1.7\Assemblies\0Harmony.dll" -p:Harmony16Path="E:\Steam\steamapps\workshop\content\294100\2009463077\1.6\Assemblies\0Harmony.dll"
```

- `RimWorldDir`：包含 `Assembly-CSharp.dll` 和 `UnityStubs.dll` 的 RimWorld 1.7 安装目录，也可通过环境变量 `RIMWORLD_DIR` 设置。
- `HarmonyPath`：Harmony Mod 的 `1.7/Assemblies/0Harmony.dll` 完整路径；默认使用 `Libraries/Harmony/1.7/0Harmony.dll`。
- `Harmony16Path`：Harmony Mod 的 `1.6/Assemblies/0Harmony.dll` 完整路径；默认使用 `Libraries/Harmony/1.6/0Harmony.dll`。
- 不要移动或修改游戏原始 DLL；项目不会将游戏参考程序集或 Harmony DLL 打包。
- 1.6 编译结果：`bin/Release/net472/EdBPrepareCarefully.dll`。
- 1.7 编译结果：`bin/Release/net10.0/EdBPrepareCarefully.dll`。
- 只编译某个版本可加 `-f net472` 或 `-f net10.0`；生成完整 dist 时不要指定单一框架。

## 生成 dist

Windows 下运行（参数与构建相同）：

```bat
dist.bat -p:RimWorldDir="E:\Steam\steamapps\common\RimWorld" -p:HarmonyPath="E:\Steam\steamapps\workshop\content\294100\2009463077\1.7\Assemblies\0Harmony.dll" -p:Harmony16Path="E:\Steam\steamapps\workshop\content\294100\2009463077\1.6\Assemblies\0Harmony.dll"
```

也可直接使用 MSBuild 发布目标：

```bat
dotnet msbuild EdBPrepareCarefully.csproj -restore -t:Dist -p:Configuration=Release -p:RimWorldDir="E:\Steam\steamapps\common\RimWorld" -p:HarmonyPath="E:\Steam\steamapps\workshop\content\294100\2009463077\1.7\Assemblies\0Harmony.dll" -p:Harmony16Path="E:\Steam\steamapps\workshop\content\294100\2009463077\1.6\Assemblies\0Harmony.dll"
```

发布目标先执行编译，成功后重新生成 `dist/EdBPrepareCarefully`：

```text
EdBPrepareCarefully/
  1.6/Assemblies/EdBPrepareCarefully.dll
  1.7/Assemblies/EdBPrepareCarefully.dll
  About/
  Common/
    Defs/
    Languages/
    Textures/
  CHANGELOG.txt
  LICENSE
  THIRD-PARTY-LICENSES
  LoadFolders.xml
```

将 `dist/EdBPrepareCarefully` 整个文件夹复制到游戏的 `Mods` 目录即可。`LoadFolders.xml` 会根据游戏版本仅加载 `1.6` 或 `1.7` 程序集，简繁中文资源由两个版本共用。

## 更新创意工坊

`Resources/About/PublishedFileId.txt` 已绑定现有条目 `3546254702`，生成 dist 时会自动复制到 `dist/EdBPrepareCarefully/About/PublishedFileId.txt`。
将完整发布目录放入游戏 `Mods`，用条目所有者的 Steam 账号在游戏中更新创意工坊即可。请勿改动此 ID，以免上传到新条目；本脚本仅构建打包，不会自动上传。

## Translations

For translation-only contributions, consider creating a standalone translation mod containing language resources, without redistributing the code assemblies.
