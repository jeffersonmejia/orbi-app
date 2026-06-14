// Nav: active link prevention + icon hover pulse
document.addEventListener('DOMContentLoaded', () => {
  // Prevent navigation when clicking already-active link
  document.querySelectorAll('.island-link.active').forEach(link => {
    link.addEventListener('click', e => e.preventDefault());
  });

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