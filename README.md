# Çizgi İzleyen Araç (Line Follower) - Fizik Simülasyonu

Unity oyun motoru ve C# kullanılarak geliştirilmiş, otonom çizgi izleyen araç simülasyonu. Araç üzerindeki sanal sensörlerin zemindeki çizgiyi algılaması ve motorlara (tekerleklere) anlık tork/dönüş açılarını iletmesi prensibiyle çalışır.

## Proje Özeti
Bu proje, fiziksel bir otonom robotun sanal ortamdaki dijital ikizini (digital twin) oluşturmak amacıyla tasarlanmıştır. Donanım maliyetleri olmadan algoritma ve fizik testlerinin yapılabilmesini sağlar.

## Teknolojiler (Tech Stack)
- **Oyun Motoru:** Unity 3D
- **Programlama Dili:** C#
- **Fizik Yönetimi:** Unity Rigidbody & WheelCollider

## Öne Çıkan Özellikler
- **Sanal Sensör Algoritması:** Zemindeki renk/çizgi farklılıklarını algılayan Raycast tabanlı sensör mimarisi.
- **Dinamik Araç Fiziği:** Gerçekçi ivmelenme, frenleme ve virajlarda merkezkaç dengesini sağlayan fizik kuralları.
- **Otonom Sürüş:** Sensörlerden gelen anlık verilere göre karar veren motor ve direksiyon kontrol algoritması.

## Kurulum ve Çalıştırma
1. Bu projeyi bilgisayarınıza klonlayın.
2. **Unity Hub** üzerinden `Add` (Ekle) butonuna basarak projenin ana klasörünü seçin.
3. Proje açıldığında `Scenes` (Sahneler) klasörü altındaki ana sahneyi açın ve üstteki **Play** butonuna basarak simülasyonu başlatın.