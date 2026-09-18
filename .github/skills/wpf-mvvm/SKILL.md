---
name: wpf-mvvm
description: |
WPF MVVMアプリケーション作成時に利用する。
ViewModel、Model、Service分割方針を定義する。
---

# WPF MVVM ガイド

## 適用条件

- WPF
- .NET Framework 4.8
- MVVM

## View

- XAMLにビジネスロジックを記述しない。
- Marginを適切に設定する。
- DataBindingを優先する。

## ViewModel

- 画面ロジックを実装する。
- Viewへの直接参照は禁止。

## Model

- 業務データを保持する。

## Service

- ファイルアクセス
- DBアクセス
- 外部サービス連携

などを担当する。

## XMLコメント

public/private問わず記述する。
