// Nav: active link prevention + manage toggle + icon hover pulse
document.addEventListener('DOMContentLoaded', () => {
  // Prevent navigation when clicking already-active link
  document.querySelectorAll('.island-link.active').forEach(link => {
    link.addEventListener('click', e => e.preventDefault());
  });

  // Manage toggle expand/collapse
  const toggle = document.querySelector('.manage-toggle');
  const nav = document.querySelector('.island-nav');
  if (toggle && nav) {
    // Start collapsed unless a manage link is active
    const hasActive = document.querySelector('.manage-links .island-link.active');
    if (hasActive) nav.classList.add('manage-open');

    toggle.addEventListener('click', () => {
      nav.classList.toggle('manage-open');
    });
  }

  // Icon hover pulse
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
});