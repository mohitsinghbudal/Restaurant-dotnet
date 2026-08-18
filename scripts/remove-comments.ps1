param(
	[string]$Root = "$(Split-Path -Parent $PSScriptRoot)"
)

function Strip-Comments {
	param([string]$code)

	$sb = New-Object System.Text.StringBuilder
	$len = $code.Length
	$i = 0

	$state = 'normal'

	while ($i -lt $len) {
		switch ($state) {
			'normal' {
				if ($i + 1 -lt $len -and $code[$i] -eq '/' -and $code[$i+1] -eq '/') {
					$state = 'line'
					$i += 2
					continue
				}
				if ($i + 1 -lt $len -and $code[$i] -eq '/' -and $code[$i+1] -eq '*') {
					$state = 'block'
					$i += 2
					continue
				}
				if ($i + 1 -lt $len -and $code[$i] -eq '@' -and $code[$i+1] -eq '"') {
					$sb.Append('@"') | Out-Null
					$state = 'verbatim'
					$i += 2
					continue
				}
				if ($code[$i] -eq '"') {
					$sb.Append('"') | Out-Null
					$state = 'string'
					$i++
					continue
				}
				if ($code[$i] -eq '''') {
					$sb.Append("'") | Out-Null
					$state = 'char'
					$i++
					continue
				}
				$sb.Append($code[$i]) | Out-Null
				$i++
			}
			'line' {
				if ($code[$i] -eq "`r") {
					$sb.Append("`r") | Out-Null
					if ($i + 1 -lt $len -and $code[$i+1] -eq "`n") { $sb.Append("`n") | Out-Null; $i += 2 } else { $i++ }
					$state = 'normal'
					continue
				}
				if ($code[$i] -eq "`n") {
					$sb.Append("`n") | Out-Null
					$i++
					$state = 'normal'
					continue
				}
				$i++
			}
			'block' {
				if ($i + 1 -lt $len -and $code[$i] -eq '*' -and $code[$i+1] -eq '/') {
					$i += 2
					$state = 'normal'
					continue
				}
				$i++
			}
			'string' {
				if ($code[$i] -eq '\\') {
					# escape sequence, keep both
					if ($i + 1 -lt $len) { $sb.Append($code[$i]) | Out-Null; $sb.Append($code[$i+1]) | Out-Null; $i += 2 } else { $sb.Append($code[$i]) | Out-Null; $i++ }
					continue
				}
				if ($code[$i] -eq '"') {
					$sb.Append('"') | Out-Null
					$i++
					$state = 'normal'
					continue
				}
				$sb.Append($code[$i]) | Out-Null
				$i++
			}
			'verbatim' {
				if ($code[$i] -eq '"') {
					if ($i + 1 -lt $len -and $code[$i+1] -eq '"') {
						# double quote inside verbatim string
						$sb.Append('""') | Out-Null
						$i += 2
						continue
					} else {
						$sb.Append('"') | Out-Null
						$i++
						$state = 'normal'
						continue
					}
				}
				$sb.Append($code[$i]) | Out-Null
				$i++
			}
			'char' {
				if ($code[$i] -eq '\\') {
					if ($i + 1 -lt $len) { $sb.Append($code[$i]); $sb.Append($code[$i+1]) | Out-Null; $i += 2 } else { $sb.Append($code[$i]) | Out-Null; $i++ }
					continue
				}
				if ($code[$i] -eq "'") {
					$sb.Append("'") | Out-Null
					$i++
					$state = 'normal'
					continue
				}
				$sb.Append($code[$i]) | Out-Null
				$i++
			}
		}
	}

	return $sb.ToString()
}

$rootPath = Resolve-Path $Root
Write-Output "Scanning: $rootPath"

$files = Get-ChildItem -Path $rootPath -Recurse -Include *.cs | Where-Object { $_.FullName -notmatch '\\bin\\|\\obj\\|\\.git\\' }

foreach ($f in $files) {
	try {
		$orig = Get-Content -Raw -Encoding UTF8 -Path $f.FullName
	} catch {
		Write-Warning "Skipping (read error): $($f.FullName)"
		continue
	}
	$new = Strip-Comments -code $orig
	if ($new -ne $orig) {
		$backup = "$($f.FullName).bak"
		Copy-Item -Path $f.FullName -Destination $backup -Force
		Set-Content -Path $f.FullName -Value $new -Encoding UTF8
		Write-Output "Updated: $($f.FullName) (backup: $backup)"
	}
}

Write-Output "Done."
