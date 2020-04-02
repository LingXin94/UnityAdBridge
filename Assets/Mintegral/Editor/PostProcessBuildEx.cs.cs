
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

#if UNITY_IPHONE

public static class PostProcessBuildEx
{
        // ios版本xcode工程维护代码
        [PostProcessBuild(176)]
        public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)
        {
            if (BuildTarget == BuildTarget.iOS)
            {
                string projPath = PBXProject.GetPBXProjectPath(path);
                PBXProject proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));

                // 获取当前项目名字
                string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

                // 对所有的编译配置设置选项
                proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
                proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

                // 添加依赖库
                proj.AddFrameworkToProject(target, "CoreGraphics.framework", false);
                proj.AddFrameworkToProject(target, "Foundation.framework", false);
                proj.AddFrameworkToProject(target, "UIKit.framework", false);
                proj.AddFrameworkToProject(target, "libsqlite3.tbd", false);
                proj.AddFrameworkToProject(target, "libz.tbd", false);
                proj.AddFrameworkToProject(target, "AdSupport.framework", false);
                proj.AddFrameworkToProject(target, "StoreKit.framework", false);
                proj.AddFrameworkToProject(target, "QuartzCore.framework", false);
                proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);
                proj.AddFrameworkToProject(target, "MobileCoreServices.framework", false);
                proj.AddFrameworkToProject(target, "Accelerate.framework", false);
                proj.AddFrameworkToProject(target, "AVFoundation.framework", false);
                proj.AddFrameworkToProject(target, "WebKit.framework", false);

                // 保存工程
                proj.WriteToFile(projPath);

                // 修改plist
                string plistPath = Path.Combine(path, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));
                PlistElementDict rootDict = plist.root;

                // 保存plist
                plist.WriteToFile(plistPath);
            }
        }
}
#endif