// Web Animations API + Skeleton loader
document.addEventListener('DOMContentLoaded', () => {
  const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // ── Skeleton → Content reveal (FB-style) ──
  const skeletonWraps = document.querySelectorAll('.skeleton-wrap');
  if (skeletonWraps.length) {
    skeletonWraps.forEach(wrap => {
      const parent = wrap.closest('.skeleton-active') || wrap.parentElement;
      parent.classList.add('skeleton-active');
    });

    setTimeout(() => {
      skeletonWraps.forEach(wrap => {
        const parent = wrap.closest('.skeleton-active') || wrap.parentElement;
        parent.classList.remove('skeleton-active');
      });
      // After skeleton, do entrance animations
      if (!prefersReduced) triggerEntranceAnimations();
    }, 500);
  } else if (!prefersReduced) {
    triggerEntranceAnimations();
  }

  // ── Entrance animations (cards stagger, hero, alert, scroll reveal) ──
  function triggerEntranceAnimations() {
    // Staggered cards
    document.querySelectorAll('.card').forEach((card, i) => {
      card.animate([
        { opacity: 0, transform: 'translateY(20px)' },
        { opacity: 1, transform: 'translateY(0)' }
      ], { duration: 400, delay: i * 100, easing: 'ease-out', fill: 'both' });
    });

    // Hero scale-in
    const hero = document.querySelector('.text-center.py-5');
    if (hero) {
      hero.animate([
        { opacity: 0, transform: 'scale(0.95)' },
        { opacity: 1, transform: 'scale(1)' }
      ], { duration: 500, easing: 'ease-out', fill: 'both' });
    }

    // Success alert
    const alert = document.querySelector('.alert-success');
    if (alert) {
      alert.animate([
        { opacity: 0, transform: 'translateX(-30px)' },
        { opacity: 1, transform: 'translateX(0)' }
      ], { duration: 350, easing: 'ease-out', fill: 'both' });
    }
  }

  // ── Scroll-triggered reveal (Intersection Observer) ──
  const revealEls = document.querySelectorAll('[data-reveal]');
  if (revealEls.length) {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        const el = entry.target;
        const delay = parseInt(el.getAttribute('data-delay') || '0') * 100;
        el.animate([
          { opacity: 0, transform: 'translateY(24px)' },
          { opacity: 1, transform: 'translateY(0)' }
        ], { duration: 400, delay, easing: 'ease-out', fill: 'both' });
        observer.unobserve(el);
      });
    }, { threshold: 0.15 });
    revealEls.forEach(el => observer.observe(el));
  }

  // ── Nav link hover — Web API pulse ──
  document.querySelectorAll('.island-link').forEach(link => {
    link.addEventListener('mouseenter', function () {
      const icon = this.querySelector('i');
      if (!icon) return;
      icon.animate([
        { transform: 'scale(1)' },
        { transform: 'scale(1.2)' },
        { transform: 'scale(1)' }
      ], { duration: 300, easing: 'ease-out' });
    });
  });

  // ── Button ripple effect ──
  document.querySelectorAll('.btn:not(.btn-sm)').forEach(btn => {
    btn.addEventListener('click', function (e) {
      const rect = this.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const y = e.clientY - rect.top;
      const ripple = document.createElement('span');
      ripple.style.cssText = `
        position: absolute; border-radius: 50%;
        background: rgba(255,255,255,0.35);
        width: 20px; height: 20px;
        left: ${x - 10}px; top: ${y - 10}px;
        pointer-events: none;
      `;
      ripple.style.position = 'absolute';
      this.style.position = 'relative';
      this.style.overflow = 'hidden';
      this.appendChild(ripple);
      ripple.animate([
        { transform: 'scale(0)', opacity: 1 },
        { transform: 'scale(8)', opacity: 0 }
      ], { duration: 500, easing: 'ease-out' }).onfinish = () => ripple.remove();
    });
  });
});
