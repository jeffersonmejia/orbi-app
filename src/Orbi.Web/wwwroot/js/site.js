// Global: Skeleton loader only (animations are CSS-based)
document.addEventListener('DOMContentLoaded', () => {
  const skeletonWraps = document.querySelectorAll('.skeleton-wrap');
  if (!skeletonWraps.length) return;

  skeletonWraps.forEach(wrap => {
    const parent = wrap.closest('.skeleton-active') || wrap.parentElement;
    parent.classList.add('skeleton-active');
  });

  setTimeout(() => {
    skeletonWraps.forEach(wrap => {
      const parent = wrap.closest('.skeleton-active') || wrap.parentElement;
      parent.classList.remove('skeleton-active');
    });
  }, 500);
});
