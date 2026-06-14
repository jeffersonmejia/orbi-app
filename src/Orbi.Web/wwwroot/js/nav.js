// Nav link hover effect
document.addEventListener('DOMContentLoaded', () => {
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
