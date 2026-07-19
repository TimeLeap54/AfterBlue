param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$packageRoot = Join-Path $ProjectRoot 'Assets\Flooded_Grounds'
$postProcessingRoot = Join-Path $packageRoot 'PostProcessing'
$minDrawerPath = Join-Path $postProcessingRoot 'Editor\PropertyDrawers\MinDrawer.cs'
$treePrefabRoot = Join-Path $packageRoot 'Prefabs\Nature\Trees'

if (-not (Test-Path -LiteralPath $packageRoot)) {
    Write-Host "Flooded Grounds is not imported: $packageRoot"
    exit 0
}

if (Test-Path -LiteralPath $minDrawerPath) {
    $text = Get-Content -LiteralPath $minDrawerPath -Raw
    $text = $text.Replace('[CustomPropertyDrawer(typeof(MinAttribute))]', '[CustomPropertyDrawer(typeof(UnityEngine.PostProcessing.MinAttribute))]')
    $text = $text.Replace('MinAttribute attribute = (MinAttribute)base.attribute;', 'UnityEngine.PostProcessing.MinAttribute attribute = (UnityEngine.PostProcessing.MinAttribute)base.attribute;')
    Set-Content -LiteralPath $minDrawerPath -Value $text -NoNewline
    Write-Host "Patched MinDrawer.cs"
}

$eventTypeMap = [ordered]@{
    'EventType.repaint' = 'EventType.Repaint'
    'EventType.layout' = 'EventType.Layout'
    'EventType.mouseDown' = 'EventType.MouseDown'
    'EventType.mouseUp' = 'EventType.MouseUp'
    'EventType.mouseDrag' = 'EventType.MouseDrag'
    'EventType.mouseMove' = 'EventType.MouseMove'
    'EventType.keyDown' = 'EventType.KeyDown'
    'EventType.keyUp' = 'EventType.KeyUp'
    'EventType.scrollWheel' = 'EventType.ScrollWheel'
}

$changed = 0
if (Test-Path -LiteralPath $postProcessingRoot) {
    Get-ChildItem -Path $postProcessingRoot -Recurse -Filter *.cs -File | ForEach-Object {
        $text = Get-Content -LiteralPath $_.FullName -Raw
        $newText = $text
        foreach ($key in $eventTypeMap.Keys) {
            $newText = $newText.Replace($key, $eventTypeMap[$key])
        }

        if ($newText -ne $text) {
            Set-Content -LiteralPath $_.FullName -Value $newText -NoNewline
            $changed++
        }
    }
}

Write-Host "Patched obsolete EventType references in $changed file(s)."

# The 2019 TreeCreator prefabs embed legacy tree shaders that render pink in URP.
# For local review scenes, point those embedded materials at URP/Lit while keeping textures.
$legacyTreeLeafShaderGuid = '56e01684409c0f740b8acfe62e330a33'
$legacyTreeBarkShaderGuid = 'db6aa165e16df4c468e10d58b72a89b0'
$urpLitShaderGuid = '933532a4fcc9baf4fa0491de14d08ed7'

$treeShaderChanged = 0
if (Test-Path -LiteralPath $treePrefabRoot) {
    Get-ChildItem -Path $treePrefabRoot -Filter *.prefab -File | ForEach-Object {
        $text = Get-Content -LiteralPath $_.FullName -Raw
        $newText = $text
        $newText = $newText.Replace("guid: $legacyTreeLeafShaderGuid", "guid: $urpLitShaderGuid")
        $newText = $newText.Replace("guid: $legacyTreeBarkShaderGuid", "guid: $urpLitShaderGuid")

        if ($newText -ne $text) {
            Set-Content -LiteralPath $_.FullName -Value $newText -NoNewline
            $treeShaderChanged++
        }
    }
}

Write-Host "Patched TreeCreator legacy shaders in $treeShaderChanged prefab(s)."
