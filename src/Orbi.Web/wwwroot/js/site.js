// Global: Skeleton loader only (animations are CSS-based)
document.addEventListener('DOMContentLoaded', () => {
  const skeletonWraps = document.querySelectorAll('.skeleton-wrap');
  if (!skeletonWraps.length) return;

  setTimeout(() => {
    skeletonWraps.forEach(wrap => wrap.classList.add('hidden'));
    document.querySelectorAll('.content-real').forEach(el => el.classList.add('revealed'));
  }, 500);
});