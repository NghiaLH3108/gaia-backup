import { useEffect } from 'react';

function App() {
  useEffect(() => {
    const counters = document.querySelectorAll('.counter');
    const observerOptions = {
      threshold: 0.5
    };

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          const targetStr = entry.target.getAttribute('data-target');
          if (!targetStr) return;
          const target = +targetStr;
          let count = 0;
          const increment = target / 50;
          
          const updateCount = () => {
            if (count < target) {
              count += Math.ceil(increment);
              (entry.target as HTMLElement).innerText = (count > target ? target : count).toString();
              setTimeout(updateCount, 30);
            }
          };
          updateCount();
          observer.unobserve(entry.target);
        }
      });
    }, observerOptions);

    counters.forEach(counter => observer.observe(counter));

    const handleScroll = () => {
      const nav = document.querySelector('nav');
      if (!nav) return;
      if (window.scrollY > 50) {
        nav.classList.add('py-4');
        nav.classList.remove('h-20');
      } else {
        nav.classList.remove('py-4');
        nav.classList.add('h-20');
      }
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  return (
    <div className="overflow-x-hidden">
      {/* TopNavBar */}
      <nav className="fixed top-0 w-full z-50 backdrop-blur-md bg-glass-fill border-b border-glass-stroke shadow-sm transition-all duration-300">
        <div className="flex justify-between items-center h-20 px-container-padding-mobile md:px-container-padding-desktop max-w-full">
          <div className="font-headline-md text-headline-md font-black tracking-tighter text-primary">GAIA</div>
          <div className="hidden md:flex gap-8 items-center">
            <a className="font-label-sm text-label-sm uppercase tracking-wider text-primary border-b-2 border-primary pb-1" href="#">Our Story</a>
            <a className="font-label-sm text-label-sm uppercase tracking-wider text-on-surface-variant hover:text-primary transition-colors" href="#">The Process</a>
            <a className="font-label-sm text-label-sm uppercase tracking-wider text-on-surface-variant hover:text-primary transition-colors" href="#">Impact</a>
            <a className="font-label-sm text-label-sm uppercase tracking-wider text-on-surface-variant hover:text-primary transition-colors" href="#">Collection</a>
          </div>
          <div className="flex items-center gap-4">
            <button className="hidden md:block bg-primary text-on-primary px-6 py-2 rounded-full font-label-sm hover:opacity-80 transition-opacity active:scale-95 duration-150">Shop Now</button>
            <button className="md:hidden text-primary">
              <span className="material-symbols-outlined">menu</span>
            </button>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="relative h-screen flex items-center justify-center overflow-hidden">
        <div className="absolute inset-0 z-0">
          <div className="w-full h-full bg-cover bg-center parallax-bg" data-alt="Hero Image" style={{ backgroundImage: "url('https://lh3.googleusercontent.com/aida-public/AB6AXuDbNB-TKNuA5bqCDPOuh-Wu74FOIc0mEtNkUxGPAzAjeb-TiodTxQNJhcqQMV4Srm5nCFZ1oxATlW1s-yNz2baVaZ5lKsMOkPIQmZ-JOekD9kuKjpY3_0IluWhTLZeWJZY2XJbcJDVUP7yVWWnGzkYhoSXsN--1OisX0G6nHryDndXIEA8a8CcUxibBctoCpcMSzLMumDCZiygeHld2hh98ld7WGO0TiJ1k36b98NjxlV4YQEecIlh4')" }}></div>
          <div className="absolute inset-0 bg-black/30"></div>
        </div>
        <div className="relative z-10 text-center px-container-padding-mobile max-w-4xl mx-auto">
          <h1 className="font-display-hero-mobile md:font-display-hero text-display-hero-mobile md:text-display-hero text-white mb-6 drop-shadow-lg">
            Tôi từng là một chiếc vỏ sầu riêng
          </h1>
          <p className="font-body-lg text-body-lg text-white/90 mb-10 max-w-2xl mx-auto">
            Hành trình từ phế phẩm nông nghiệp đến kiệt tác bền vững. GAIA kiến tạo tương lai từ những giá trị bị bỏ quên.
          </p>
          <div className="flex flex-col md:flex-row gap-4 justify-center">
            <button className="bg-primary text-on-primary px-8 py-4 rounded-full font-headline-md hover:shadow-[0_0_20px_rgba(46,125,50,0.4)] transition-all">Khám phá ngay</button>
            <button className="backdrop-blur-md bg-white/10 border-2 border-white text-white px-8 py-4 rounded-full font-headline-md hover:bg-white/20 transition-all">Xem quy trình</button>
          </div>
        </div>
        <div className="absolute bottom-10 left-1/2 -translate-x-1/2 animate-bounce text-white cursor-pointer">
          <span className="material-symbols-outlined text-4xl">keyboard_arrow_down</span>
        </div>
      </section>

      {/* Product Story Section */}
      <section className="py-section-margin px-container-padding-mobile md:px-container-padding-desktop bg-surface">
        <div className="max-w-7xl mx-auto grid grid-cols-1 md:grid-cols-2 gap-16 items-center">
          <div className="order-2 md:order-1">
            <div className="inline-block px-4 py-1 rounded-full bg-surface-accent/30 text-primary font-label-sm mb-6">CÂU CHUYỆN SẢN PHẨM</div>
            <h2 className="font-headline-lg text-headline-lg text-on-background mb-6">Vẻ đẹp từ sự tái sinh</h2>
            <div className="space-y-6">
              <p className="font-body-md text-body-md text-on-surface-variant leading-relaxed">
                Tại GAIA, chúng tôi tin rằng mỗi mảnh phế thải đều mang trong mình một linh hồn mới. Những chiếc vỏ sầu riêng thô kệch, sau hàng trăm giờ nghiên cứu và chế tác thủ công, đã trở thành những tấm vật liệu hữu cơ mang vẻ đẹp của nghệ thuật đương đại.
              </p>
              <p className="font-body-md text-body-md text-on-surface-variant leading-relaxed border-l-4 border-primary pl-6 py-2 italic bg-surface-container-low/50">
                "Không chỉ là sản phẩm, đó là lời cam kết của chúng tôi với hành tinh xanh và sự tôn vinh đôi bàn tay khéo léo của nghệ nhân Việt."
              </p>
            </div>
            <div className="mt-10 flex gap-12">
              <div>
                <div className="font-headline-md text-headline-md text-primary font-bold">100%</div>
                <div className="font-label-sm text-label-sm text-on-surface-variant">Hữu cơ</div>
              </div>
              <div>
                <div className="font-headline-md text-headline-md text-primary font-bold">0%</div>
                <div className="font-label-sm text-label-sm text-on-surface-variant">Nhựa độc hại</div>
              </div>
              <div>
                <div className="font-headline-md text-headline-md text-primary font-bold">48h</div>
                <div className="font-label-sm text-label-sm text-on-surface-variant">Chế tác thủ công</div>
              </div>
            </div>
          </div>
          <div className="order-1 md:order-2">
            <div className="relative group">
              <div className="absolute -inset-4 bg-primary/5 rounded-3xl -z-10 group-hover:bg-primary/10 transition-colors"></div>
              <img className="w-full rounded-2xl shadow-xl object-cover aspect-[4/5]" alt="Story Image" src="https://lh3.googleusercontent.com/aida-public/AB6AXuCHvkBOjPA2U0YHqOemJSnYGkzAAGMqfY2GBM4GYsw1hXny47geEkAMUAO6MfjgKQCXR0vBQj52Y9q6wkNV_pknG62uQIwk42fzOy1208eGl7afoV5uCLAL7lMRHd4UPsr4is8N_m1Mh4PqdMxLm2aPfxfJ-cf7kZGiYun7k15VlmOpu1iYjg3CH9zni2Vd5JF1mUlc46mQZ8BCVvpFFUWwW-iLO88Ok_RGZLIovDjHMpL7Fhh1vDZ3" />
            </div>
          </div>
        </div>
      </section>

      {/* Impact Metrics Section */}
      <section className="py-section-margin bg-primary-container text-on-primary-container relative overflow-hidden">
        <div className="absolute top-0 right-0 w-64 h-64 bg-secondary-container/20 rounded-full blur-[100px]"></div>
        <div className="absolute bottom-0 left-0 w-96 h-96 bg-on-primary-container/10 rounded-full blur-[120px]"></div>
        <div className="max-w-7xl mx-auto px-container-padding-mobile md:px-container-padding-desktop relative z-10">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-12 text-center">
            <div className="space-y-2">
              <span className="material-symbols-outlined text-5xl mb-4 opacity-80" data-weight="fill">recycling</span>
              <div className="font-display-hero text-display-hero font-bold counter" data-target="15000">0</div>
              <div className="font-headline-md text-headline-md opacity-90">Tấn rác thải đã tái chế</div>
            </div>
            <div className="space-y-2">
              <span className="material-symbols-outlined text-5xl mb-4 opacity-80" data-weight="fill">co2</span>
              <div className="font-display-hero text-display-hero font-bold counter" data-target="2450">0</div>
              <div className="font-headline-md text-headline-md opacity-90">CO2 đã giảm thiểu (Tấn)</div>
            </div>
            <div className="space-y-2">
              <span className="material-symbols-outlined text-5xl mb-4 opacity-80" data-weight="fill">groups</span>
              <div className="font-display-hero text-display-hero font-bold counter" data-target="320">0</div>
              <div className="font-headline-md text-headline-md opacity-90">Cộng đồng nghệ nhân</div>
            </div>
          </div>
        </div>
      </section>

      {/* Product Gallery */}
      <section className="py-section-margin px-container-padding-mobile md:px-container-padding-desktop">
        <div className="max-w-7xl mx-auto mb-16 flex flex-col md:flex-row justify-between items-end gap-6">
          <div className="max-w-xl">
            <h2 className="font-headline-lg text-headline-lg text-on-background mb-4">Bộ sưu tập GAIA Signature</h2>
            <p className="font-body-md text-body-md text-on-surface-variant">Khám phá những thiết kế tối giản, hiện đại mang hơi thở của đất mẹ.</p>
          </div>
          <a className="text-primary font-label-sm border-b-2 border-primary pb-1 hover:opacity-70 transition-opacity" href="#">Xem tất cả sản phẩm</a>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-gutter">
          <div className="lg:col-span-2 group">
            <div className="relative overflow-hidden rounded-3xl bg-surface-container-low aspect-[16/9]">
              <img className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" src="https://lh3.googleusercontent.com/aida/AP1WRLuZFDqCn1evgBArvGKdXMN8vuBIL_R0eAccW21KY1mRt9nSsMR11gCXOT6sKGFEx5WhEhHFmhzylHrhk9TEWKvr5w0VbTQJ0HzqLQBAxVR5velUl3kmZdRMuQK8ENA15m8OxwECmtjfL0I32RUatql1tErTdKPPdYOnloFPRWLIGiN-v4XKOSCO20o3XqvCunRt-5D2xeily_toT0nc7i8vO6twrymU-qd2bhbltjI1HaP3pG87Hmbtl_s" />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
              <div className="absolute bottom-8 left-8 text-white translate-y-4 group-hover:translate-y-0 opacity-0 group-hover:opacity-100 transition-all duration-300">
                <h3 className="font-headline-lg text-headline-lg">Hộp Lưu Trữ Organic S01</h3>
                <p className="font-body-md text-body-md mb-4">Tinh tế - Bền bỉ - Hoàn toàn tự nhiên</p>
                <button className="bg-white text-primary px-6 py-2 rounded-full font-label-sm">Mua ngay</button>
              </div>
            </div>
          </div>
          <div className="glass-card p-6 rounded-3xl custom-shadow flex flex-col group">
            <div className="relative overflow-hidden rounded-2xl aspect-square mb-6">
              <img className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110" alt="Product Notebook" src="https://lh3.googleusercontent.com/aida-public/AB6AXuCjzdeaBwgvqAyrrfXltI_POXzUmMllfoVRotEJUZ3XCqIFFmkZRyKaGwSilWjYAMUA0UcSwORBojLyXaNERQRuwA5fERS_5v0Hp6mKpzoJfOtLXubCXIkX9FEQyF2mQoufRTMuASpL2Y-y03dWDdqsz6gCfToL5_b76AQJU89WFenOuYJrtz404oHpGtT02ZcLMWJ8KPOcB6k96XvAESE79dESPQTTqQ-40cwie25iv-FsKv_Zo2fu" />
            </div>
            <h3 className="font-headline-md text-headline-md text-on-background mb-2">Sổ Tay GAIA Earth</h3>
            <p className="font-body-md text-body-md text-on-surface-variant mb-6">Sản phẩm bán chạy nhất quý 4/2024</p>
            <div className="mt-auto flex justify-between items-center">
              <span className="font-headline-md text-headline-md text-primary">450.000đ</span>
              <button className="w-10 h-10 rounded-full border border-primary text-primary flex items-center justify-center hover:bg-primary hover:text-white transition-colors">
                <span className="material-symbols-outlined">add_shopping_cart</span>
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* Artisanal Process Section */}
      <section className="py-section-margin bg-surface-container-lowest">
        <div className="max-w-7xl mx-auto px-container-padding-mobile md:px-container-padding-desktop">
          <div className="text-center max-w-2xl mx-auto mb-16">
            <h2 className="font-headline-lg text-headline-lg text-on-background mb-4">Nghệ thuật của sự tỉ mỉ</h2>
            <p className="font-body-md text-body-md text-on-surface-variant">Mỗi sản phẩm GAIA đi qua 12 công đoạn kiểm duyệt khắt khe dưới bàn tay của các nghệ nhân làng nghề truyền thống.</p>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 md:gap-6">
            <div className="col-span-2 row-span-2 overflow-hidden rounded-3xl h-[600px] group">
              <img className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-1000" src="https://lh3.googleusercontent.com/aida/AP1WRLtymaBdernxFpDeCyoRVZlqndT1mRJ7V7uAVrtXhkZ-h7MWoxive5J_k3H4_-foBWAamg8YPiExCRcUm0BRfh9_OVvQLcXxKtsrz0AoxXEgfGs0nk4Pkg3uquXOHnoWDzIMjTHqyQo8Egmug1-JJTmhgX5DZbUuCgn4Q3xZpxsYvCabtPn3MgmkA4EmRfdgChsB0IMH9_ahu7OhgMYIKBzH3ryysAfzIccyoOl2CyF19LZULIe3tB9bO_o" />
              <div className="absolute bottom-6 left-6 p-6 glass-card rounded-2xl max-w-[80%]">
                <div className="font-label-sm text-primary mb-1">Giai đoạn 04: Chế tác</div>
                <div className="font-headline-md text-headline-md">Đan lát thủ công truyền thống</div>
              </div>
            </div>
            <div className="overflow-hidden rounded-3xl h-[290px] relative group">
              <img className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700" alt="Weaving" src="https://lh3.googleusercontent.com/aida-public/AB6AXuA666OriWH8LEQofLzau_4sbxIVf4NN6CAIFCjQ63Zz5wRHhu9kTSwYtm4z7PyT7uAfoQjfphrTSdKA85dQ1q3qevIFKhQcKzBGTrwwvB5-qxHsnB_6FKXT_wXITh2TdcmI2KjkduI1TOypTqZI0SAUvCJ45-r--4Ng9BpsGXPHbhY4xsmYqmk7b7JeIsw6kZmbA01fV-WJjpQK7cdO9H2nIzfCxK9hLh04VXl15dYx6irRlDDVH2Hg" />
            </div>
            <div className="overflow-hidden rounded-3xl h-[290px] relative group">
              <img className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700" alt="Laboratory" src="https://lh3.googleusercontent.com/aida-public/AB6AXuBjva8EaozKw9a30cP41Vs4KjT-qqY5zrH_1LEG8h-ld0felrNNeZISM2LD_jOkfo5b5pv5DjJLqyloTWkL6eNoTJoGB62Svb0MvMxdxZz10xcBvrAtTwmq8PbftUuEBS6HMKJJQF2SrB3SyXzVyAFJSGzpdNo_jcP1aHd3XTNKtMZjg4kGoycJ_N5J4oqySndZ101vQxHnBnyHupYG-xe5QOo7t_uZFvOa_b6pot_WrSD32yYn4fLq" />
            </div>
            <div className="col-span-2 overflow-hidden rounded-3xl h-[285px] relative group">
              <img className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700" alt="Factory" src="https://lh3.googleusercontent.com/aida-public/AB6AXuAQy7Gw5XBZ8kcJBxaRYN-vO01hqmvYF8jGfbDQRhEny9ggWwQpiMqs4h7b5cmuJfONAcqsRRLYYVuei9q6PVvFrKHRhASJCFVS3OBU5lbXQqhoBUp5kzVle7axgKSEl5nrsnVLW0sD3-JhLt56fuMPoOQsmfY9dMaSvmN0o0i5-4Qap2OzYzp-IKcBDwkRUVvDmtnY96WEvK7f18Bt7YZustdi-D_tbvjvipp_ETdyRe6cH8lk8-7J" />
            </div>
          </div>
        </div>
      </section>

      {/* Video Experience Section */}
      <section className="py-section-margin relative h-[80vh] flex items-center justify-center">
        <div className="absolute inset-0 z-0">
          <div className="w-full h-full bg-cover bg-center" style={{ backgroundImage: "url('https://lh3.googleusercontent.com/aida-public/AB6AXuA0v5M--n7QPSqL0ASwYuISXimIVtNvpv9HgKDcFsXUdmkZRKsYvERXoZZrOcs8emnrXCwLANsBZIAoS-NPlB9iE9cSDh-p4mEzluvrE9jF2FJC8iNq5f4bp8TEAByqTmXQZpCvSdvZlXPh7UUW_eCBa04Ph70yMai91_j__zLRgX2HIRwe5c6GsnJU0bI2gjN5-o1EK7iyJz8DW4r9KmpM-ixbIHRNewDs6cO3tuKDOIL-1CexRSe1')" }}></div>
          <div className="absolute inset-0 bg-black/40 backdrop-blur-[2px]"></div>
        </div>
        <div className="relative z-10 text-center text-white px-container-padding-mobile">
          <button className="w-24 h-24 md:w-32 md:h-32 rounded-full bg-white/20 border-2 border-white/40 flex items-center justify-center group hover:bg-primary transition-all duration-300 mb-8 mx-auto">
            <span className="material-symbols-outlined text-6xl group-hover:scale-110 transition-transform" data-weight="fill">play_arrow</span>
          </button>
          <h2 className="font-display-hero-mobile md:font-display-hero text-display-hero-mobile md:text-display-hero mb-4">Hành trình tái sinh</h2>
          <p className="font-body-lg text-body-lg text-white/80 max-w-xl mx-auto">Khám phá quy trình từ vỏ trái cây đến vật liệu công nghệ cao trong 3 phút phim điện ảnh.</p>
        </div>
      </section>

      {/* Footer */}
      <footer className="bg-surface-container-lowest border-t border-outline-variant w-full py-section-margin">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-gutter px-container-padding-mobile md:px-container-padding-desktop max-w-7xl mx-auto">
          <div className="space-y-6">
            <div className="font-headline-lg text-headline-lg font-bold text-primary">GAIA</div>
            <p className="font-body-md text-body-md text-on-surface-variant">Kiến tạo tương lai bền vững thông qua công nghệ vật liệu hữu cơ và tinh hoa thủ công Việt.</p>
            <div className="flex gap-4">
              <a className="w-10 h-10 rounded-full border border-outline-variant flex items-center justify-center hover:bg-primary hover:text-white transition-all" href="#"><span className="material-symbols-outlined">face_nod</span></a>
              <a className="w-10 h-10 rounded-full border border-outline-variant flex items-center justify-center hover:bg-primary hover:text-white transition-all" href="#"><span className="material-symbols-outlined">camera</span></a>
              <a className="w-10 h-10 rounded-full border border-outline-variant flex items-center justify-center hover:bg-primary hover:text-white transition-all" href="#"><span class="material-symbols-outlined">youtube_activity</span></a>
            </div>
          </div>
          <div className="space-y-4">
            <h4 className="font-label-sm text-label-sm text-primary uppercase">Tài nguyên</h4>
            <ul className="space-y-3 font-body-md text-body-md text-on-surface-variant">
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Traceability Portal</a></li>
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Material Science</a></li>
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Impact Report</a></li>
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Privacy Policy</a></li>
            </ul>
          </div>
          <div className="space-y-4">
            <h4 className="font-label-sm text-label-sm text-primary uppercase">Cửa hàng</h4>
            <ul className="space-y-3 font-body-md text-body-md text-on-surface-variant">
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Sản phẩm mới</a></li>
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Best Sellers</a></li>
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Dành cho doanh nghiệp</a></li>
              <li><a className="hover:text-primary hover:underline decoration-primary underline-offset-4 transition-all" href="#">Hợp tác đại lý</a></li>
            </ul>
          </div>
          <div className="space-y-6">
            <h4 className="font-label-sm text-label-sm text-primary uppercase">Đăng ký nhận tin</h4>
            <p className="font-body-md text-body-md text-on-surface-variant">Nhận cập nhật về các bộ sưu tập mới và báo cáo tác động hàng tháng.</p>
            <div className="flex">
              <input className="bg-surface-accent/20 border-none rounded-l-lg px-4 py-3 w-full focus:ring-2 focus:ring-primary outline-none" placeholder="Email của bạn" type="email" />
              <button className="bg-primary text-on-primary px-6 rounded-r-lg hover:opacity-90 transition-opacity">Gửi</button>
            </div>
          </div>
        </div>
        <div className="max-w-7xl mx-auto px-container-padding-mobile md:px-container-padding-desktop mt-16 pt-8 border-t border-outline-variant text-center md:text-left">
          <p className="font-body-md text-body-md text-on-surface-variant">© 2024 GAIA Sustainable Tech-Organic. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
}

export default App;
