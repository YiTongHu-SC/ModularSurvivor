using System.Diagnostics;
using System.IO;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace GameEditor.Editor.Tools
{
    public static class BatchScriptRunner
    {
        // 预设的bat脚本路径
        private const string BAT_SCRIPT_PATH = @"Run\gen_client.bat";

        [MenuItem("Tools/Excel导出")]
        private static void RunBuildScript()
        {
            ExecuteBatScript();
        }

        private static void ExecuteBatScript()
        {
            // 构建完整路径：项目根目录 + Run/gen_client.bat
            string projectRoot = Path.GetDirectoryName(UnityEngine.Application.dataPath);
            if (string.IsNullOrEmpty(projectRoot))
            {
                Debug.LogError("无法获取项目根目录");
                return;
            }

            string fullPath = Path.Combine(projectRoot, BAT_SCRIPT_PATH);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"脚本文件不存在: {fullPath}");
                EditorUtility.DisplayDialog("错误", $"找不到脚本文件:\n{fullPath}", "确定");
                return;
            }

            try
            {
                string workingDir = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrEmpty(workingDir))
                {
                    workingDir = projectRoot;
                }

                // 读取bat文件内容，处理BOM问题
                string batContent = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);

                // 创建临时bat文件（无BOM），添加UTF-8设置
                string tempBatPath = Path.Combine(Path.GetTempPath(), "temp_gen_client.bat");
                string enhancedBatContent = $@"@echo off
chcp 65001 > nul
{batContent}";
                File.WriteAllText(tempBatPath, enhancedBatContent, new System.Text.UTF8Encoding(false));

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{tempBatPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                Debug.Log($"=== 开始执行脚本 ===");
                Debug.Log($"原始脚本: {fullPath}");
                Debug.Log($"临时BAT脚本: {tempBatPath}");
                Debug.Log($"工作目录: {workingDir}");
                Debug.Log($"===================");

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;

                    // 启动进程
                    if (!process.Start())
                    {
                        Debug.LogError("无法启动进程");
                        return;
                    }

                    // 同步读取所有输出
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // 等待进程结束
                    process.WaitForExit();

                    // 输出结果
                    if (!string.IsNullOrEmpty(output))
                    {
                        // 按行分割输出，逐行显示
                        string[] outputLines = output.Split(new[] { '\r', '\n' },
                            System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in outputLines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Debug.Log($"[脚本输出] {line}");
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        // 按行分割错误，逐行显示
                        string[] errorLines = error.Split(new[] { '\r', '\n' },
                            System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in errorLines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                Debug.LogError($"[脚本错误] {line}");
                            }
                        }
                    }

                    Debug.Log($"=== 脚本执行完成 ===");
                    Debug.Log($"退出码: {process.ExitCode}");
                    if (process.ExitCode == 0)
                    {
                        Debug.Log("脚本执行成功！");
                    }
                    else
                    {
                        Debug.LogWarning($"脚本执行可能有问题，退出码: {process.ExitCode}");
                    }

                    Debug.Log($"===================");
                }

                // 清理临时文件
                try
                {
                    if (File.Exists(tempBatPath))
                    {
                        File.Delete(tempBatPath);
                    }
                }
                catch
                {
                    // 忽略删除临时文件的错误
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"执行脚本失败: {e.Message}");
                EditorUtility.DisplayDialog("执行错误", $"执行脚本时出错:\n{e.Message}", "确定");
            }
        }
    }
}