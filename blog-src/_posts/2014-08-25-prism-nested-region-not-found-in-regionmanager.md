---
layout: post
title: 解決Prism中嵌套的Region在RegionManager中找不到的問題 
permalink: prism-nested-region-not-found-in-regionmanager
tags: [.Net, Prism]
comments: true
published: true
---

示意圖
![示意圖](https://farm6.staticflickr.com/5576/15167455132_c62738dbc4_o.png "示意圖")

## 解釋

Region A裝載ModuleA，RegionB裝載ModuleB，Region C裝載Module C
其中，Module A內部還有 Region A1和A2，現在需要Region A1的內容導航爲其他內容。
<!--more-->
{% highlight c# %}
this.regionManager.RegisterViewWithRegion(RegionA1, typeof(RegionA1Content1));
{% endhighlight %}
頁面可以正常顯示，RegionA1Content顯示在RegionA1中，但是用RequestNavigate進行導航時失敗，經查是由於RegionManager不存在RegionWizardMainContent這個Region。

導航語句爲：
{% highlight c# %}
this.regionManager.RequestNavigate(RegionA1, "RegionA1Content2");
{% endhighlight %}
上一句是失敗的。

## 解決方法

使用RegionManager.SetRegionManager：
{% highlight c# %}
RegionManager.SetRegionManager(this.regionA1Region, regionManager); 
{% endhighlight %}

