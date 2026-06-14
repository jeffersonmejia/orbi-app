// Button ripple effect
document.addEventListener('DOMContentLoaded', () => {
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
