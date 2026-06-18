// Nav: dropdown toggles + active link prevention + icon hover pulse
document.addEventListener('DOMContentLoaded', () => {
  const closeDropdowns = () => {
    document.querySelectorAll('.nav-dropdown.open').forEach(d => d.classList.remove('open'));
  };

  // Prevent navigation when clicking already-active link
  document.querySelectorAll('.island-link.active').forEach(link => {
    if (!link.closest('.dropdown-toggle')) {
      link.addEventListener('click', e => e.preventDefault());
    }
  });

  // Dropdown toggles
  document.querySelectorAll('.dropdown-toggle').forEach(toggle => {
    const dropdown = toggle.closest('.nav-dropdown');

    toggle.addEventListener('click', e => {
      e.preventDefault();
      if (dropdown) {
        // Close siblings
        document.querySelectorAll('.nav-dropdown.open').forEach(d => {
          if (d !== dropdown) d.classList.remove('open');
        });
        dropdown.classList.toggle('open');
      }
    });
  });

  document.querySelectorAll('.dropdown-menu .island-link').forEach(link => {
    link.addEventListener('click', closeDropdowns);
  });

  // Close dropdowns when clicking outside
  document.addEventListener('click', e => {
    if (!e.target.closest('.nav-dropdown')) {
      closeDropdowns();
    }
  });

  document.addEventListener('keydown', e => {
    if (e.key === 'Escape') closeDropdowns();
  });

  // Icon hover pulse (only on non-touch devices to avoid sticky hover)
  if (!('ontouchstart' in window)) {
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
  }
});
