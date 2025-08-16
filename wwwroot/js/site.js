$(document).ready(function () {
    // Inicializar tooltips de Bootstrap
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-hide alerts después de 5 segundos
    setTimeout(function () {
        $('.alert-dismissible').fadeOut('slow');
    }, 5000);

    // Smooth scrolling para enlaces internos
    $('a[href^="#"]').on('click', function (event) {
        var target = $(this.getAttribute('href'));
        if (target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });

    // Confirmar acciones de eliminación
    $('.btn-danger[data-action="delete"]').on('click', function (e) {
        if (!confirm('¿Está seguro de que desea eliminar este elemento?')) {
            e.preventDefault();
        }
    });

    // Buscar libros en tiempo real
    $('#searchInput').on('input', function () {
        var searchTerm = $(this).val().toLowerCase();

        $('.book-card').each(function () {
            var bookTitle = $(this).find('.book-title').text().toLowerCase();
            var bookAuthor = $(this).find('.book-author').text().toLowerCase();
            var bookCategory = $(this).find('.book-category').text().toLowerCase();

            if (bookTitle.includes(searchTerm) ||
                bookAuthor.includes(searchTerm) ||
                bookCategory.includes(searchTerm)) {
                $(this).closest('.col-lg-4, .col-md-6').show();
            } else {
                $(this).closest('.col-lg-4, .col-md-6').hide();
            }
        });

        // Mostrar mensaje si no hay resultados
        var visibleCards = $('.book-card:visible').length;
        if (visibleCards === 0 && searchTerm.length > 0) {
            if ($('#noResultsMessage').length === 0) {
                $('.row').append('<div id="noResultsMessage" class="col-12 text-center py-5">' +
                    '<i class="fas fa-search fa-3x text-muted mb-3"></i>' +
                    '<h4 class="text-muted">No se encontraron resultados</h4>' +
                    '<p class="text-muted">Intenta con otros términos de búsqueda</p>' +
                    '</div>');
            }
        } else {
            $('#noResultsMessage').remove();
        }
    });

    // Animación para contadores
    $('.availability-number, .text-primary, .text-success, .text-warning, .text-danger').each(function () {
        var $this = $(this);
        var countTo = parseInt($this.text());

        if (!isNaN(countTo)) {
            $({ countNum: 0 }).animate({
                countNum: countTo
            }, {
                duration: 2000,
                easing: 'swing',
                step: function () {
                    $this.text(Math.floor(this.countNum));
                },
                complete: function () {
                    $this.text(this.countNum);
                }
            });
        }
    });

    // Loading state para formularios
    $('form').on('submit', function () {
        var $submitBtn = $(this).find('button[type="submit"]');
        var originalText = $submitBtn.html();

        $submitBtn.prop('disabled', true);
        $submitBtn.html('<i class="fas fa-spinner fa-spin me-2"></i>Procesando...');

        // Restaurar después de 10 segundos como fallback
        setTimeout(function () {
            $submitBtn.prop('disabled', false);
            $submitBtn.html(originalText);
        }, 10000);
    });

    // Validación en tiempo real para formularios
    $('.form-control').on('blur', function () {
        validateField($(this));
    });

    function validateField($field) {
        var value = $field.val().trim();
        var fieldName = $field.attr('name');
        var isValid = true;
        var message = '';

        // Validaciones específicas
        switch (fieldName) {
            case 'Email':
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    isValid = false;
                    message = 'Formato de email inválido';
                }
                break;
            case 'Password':
                if (value.length < 6) {
                    isValid = false;
                    message = 'La contraseña debe tener al menos 6 caracteres';
                }
                break;
            case 'ConfirmarPassword':
                var password = $('input[name="Password"]').val();
                if (value !== password) {
                    isValid = false;
                    message = 'Las contraseñas no coinciden';
                }
                break;
        }

        // Aplicar estilos de validación
        if (value === '') {
            $field.removeClass('is-valid is-invalid');
        } else if (isValid) {
            $field.removeClass('is-invalid').addClass('is-valid');
            $field.siblings('.invalid-feedback').hide();
        } else {
            $field.removeClass('is-valid').addClass('is-invalid');
            if ($field.siblings('.invalid-feedback').length === 0) {
                $field.after('<div class="invalid-feedback">' + message + '</div>');
            } else {
                $field.siblings('.invalid-feedback').text(message).show();
            }
        }
    }

    // Modo oscuro toggle (opcional)
    $('#darkModeToggle').on('click', function () {
        $('body').toggleClass('dark-mode');
        localStorage.setItem('darkMode', $('body').hasClass('dark-mode'));

        var icon = $(this).find('i');
        if ($('body').hasClass('dark-mode')) {
            icon.removeClass('fa-moon').addClass('fa-sun');
        } else {
            icon.removeClass('fa-sun').addClass('fa-moon');
        }
    });

    // Cargar preferencia de modo oscuro
    if (localStorage.getItem('darkMode') === 'true') {
        $('body').addClass('dark-mode');
        $('#darkModeToggle i').removeClass('fa-moon').addClass('fa-sun');
    }

    // Lazy loading para imágenes
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    imageObserver.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src]').forEach(img => {
            imageObserver.observe(img);
        });
    }

    // Scroll to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('#scrollToTop').fadeIn();
        } else {
            $('#scrollToTop').fadeOut();
        }
    });

    $('#scrollToTop').on('click', function () {
        $('html, body').animate({ scrollTop: 0 }, 600);
        return false;
    });

    // Añadir botón scroll to top si no existe
    if ($('#scrollToTop').length === 0) {
        $('body').append(
            '<button id="scrollToTop" class="btn btn-primary rounded-circle" ' +
            'style="position: fixed; bottom: 20px; right: 20px; width: 50px; height: 50px; display: none; z-index: 1000;">' +
            '<i class="fas fa-arrow-up"></i></button>'
        );
    }

    // Efectos de hover para tarjetas
    $('.book-card, .menu-btn').hover(
        function () {
            $(this).find('i').addClass('animated-icon');
        },
        function () {
            $(this).find('i').removeClass('animated-icon');
        }
    );

    // Confirmación para cambios importantes
    $('.estado-select').on('change', function () {
        var $select = $(this);
        var newValue = $select.val();
        var oldValue = $select.data('old-value') || $select.find('option:first').val();

        if (newValue !== oldValue) {
            var libro = $select.closest('tr').find('td:first').text().trim();
            if (!confirm(`¿Confirma el cambio de estado para "${libro}"?`)) {
                $select.val(oldValue);
                return false;
            }
        }

        $select.data('old-value', newValue);
    });

    // Inicializar valores antiguos para selects de estado
    $('.estado-select').each(function () {
        $(this).data('old-value', $(this).val());
    });
});

// Funciones utilitarias
function showAlert(message, type = 'info') {
    var alertClass = 'alert-' + type;
    var iconClass = type === 'success' ? 'fa-check-circle' :
        type === 'danger' ? 'fa-exclamation-triangle' :
            type === 'warning' ? 'fa-exclamation-circle' : 'fa-info-circle';

    var alert = $('<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
        '<i class="fas ' + iconClass + ' me-2"></i>' + message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>');

    $('.main-container .container').prepend(alert);

    setTimeout(function () {
        alert.fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);
}

function formatDate(date) {
    return new Date(date).toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    });
}

function formatDateTime(date) {
    return new Date(date).toLocaleString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// CSS adicional para animaciones
$('<style>').text(`
    .animated-icon {
        animation: bounce 0.6s ease-in-out;
    }
    
    @keyframes bounce {
        0%, 20%, 60%, 100% { transform: translateY(0); }
        40% { transform: translateY(-10px); }
        80% { transform: translateY(-5px); }
    }
    
    .lazy {
        opacity: 0;
        transition: opacity 0.3s;
    }
    
    .lazy.loaded {
        opacity: 1;
    }
    
    .dark-mode {
        background-color: #1a1a1a;
        color: #ffffff;
    }
    
    .dark-mode .card {
        background-color: #2d2d2d;
        border-color: #404040;
    }
    
    .dark-mode .navbar {
        background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%) !important;
    }
    
    .loading-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0,0,0,0.5);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 9999;
    }
    
    .loading-spinner {
        color: white;
        font-size: 2rem;
    }
`).appendTo('head');