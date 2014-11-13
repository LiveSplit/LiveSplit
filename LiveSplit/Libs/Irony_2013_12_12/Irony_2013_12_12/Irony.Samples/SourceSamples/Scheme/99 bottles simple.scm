(define (bottles n)
   (display n) (display " bottles of beer"))

(define (beer n)
    (if (> n 0)
        (begin
          (bottles n) (display " on the wall") (newline)
          (bottles n) (display " on the wall") (newline)
          (display "Take one down, pass it around") (newline)
          (bottles (- n 1)) (display " on the wall") (newline)
          (newline)
          (beer (- n 1)))))

(beer 100)