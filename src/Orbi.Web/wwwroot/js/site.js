// Global: Skeleton loader + Entrance animations + Scroll reveal
document.addEventListener('DOMContentLoaded', () => {
  const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  // ── Skeleton → Content reveal ──
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
      if (!prefersReduced) triggerEntranceAnimations();
    }, 500);
  } else if (!prefersReduced) {
    triggerEntranceAnimations();
  }

  function triggerEntranceAnimations() {
    document.querySelectorAll('.card').forEach((card, i) => {
      card.animate([
        { opacity: 0, transform: 'translateY(20px)' },
        { opacity: 1, transform: 'translateY(0)' }
      ], { duration: 400, delay: i * 100, easing: 'ease-out', fill: 'both' });
    });

    const hero = document.querySelector('.text-center.py-5');
    if (hero) {
      hero.animate([
        { opacity: 0, transform: 'scale(0.95)' },
        { opacity: 1, transform: 'scale(1)' }
      ], { duration: 500, easing: 'ease-out', fill: 'both' });
    }

    const alert = document.querySelector('.alert-success');
    if (alert) {
      alert.animate([
        { opacity: 0, transform: 'translateX(-30px)' },
        { opacity: 1, transform: 'translateX(0)' }
      ], { duration: 350, easing: 'ease-out', fill: 'both' });
    }
  }

  // ── Scroll-triggered reveal ──
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
});
